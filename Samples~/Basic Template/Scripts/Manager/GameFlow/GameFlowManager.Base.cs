using System.Collections.Generic;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public enum UpdatePriority
    {
        VeryHigh,
        High,
        Normal,
        Low,
        VeryLow,
    }
    
    public partial class GameFlowManager : SingletonMonoBehaviour<GameFlowManager>
    {
        private class PrioritizedUpdateHandler
        {
            public UpdateCallback UpdateCallback { get; private set; }
            public UpdatePriority Priority { get; private set; }

            public PrioritizedUpdateHandler(UpdateCallback callback, UpdatePriority priority)
            {
                UpdateCallback = callback;
                Priority = priority;
            }
        }
        
        public void Initialize()
        {
            _updateHandlers = new();
            _lateUpdateHandlers= new();
            _stateQueue= new();
            _currentState = null;
            _updateSpeed = 1.0f;
            _isPause = false;

            AddUpdateHandler(ManagedUpdate);
        }

        public delegate void UpdateCallback(float dt);

        private bool _isPause;
        private float _updateSpeed = 1.0f;
        
        #region Unity Event Method

        private List<PrioritizedUpdateHandler> _updateHandlers;
        private bool isDirtyFlagOfUpdateHandler;
        private void Update()
        {
            if (_isPause)
            {
                return;
            }

            if (isDirtyFlagOfUpdateHandler)
            {
                _updateHandlers.Sort((a, b) => b.Priority - a.Priority);
                isDirtyFlagOfUpdateHandler = false;
            }

            foreach (var handler in _updateHandlers)
            {
                handler.UpdateCallback.Invoke(Time.deltaTime * _updateSpeed);
            }
        }

        private List<PrioritizedUpdateHandler> _lateUpdateHandlers;
        private bool isDirtyFlagOfLateUpdateHandler;
        private void LateUpdate()
        {
            if (_isPause)
            {
                return;
            }

            if (isDirtyFlagOfLateUpdateHandler)
            {
                _lateUpdateHandlers.Sort((a, b) => b.Priority - a.Priority);
                isDirtyFlagOfLateUpdateHandler = false;
            }

            foreach (var handler in _lateUpdateHandlers)
            {
                handler.UpdateCallback.Invoke(Time.deltaTime * _updateSpeed);
            }
        }
        #endregion

        #region State Flow Control

        private Queue<StateBase> _stateQueue;
        private StateBase _currentState;

        private void ManagedUpdate(float dt)
        {
            if (_currentState == null)
            {
                while (_stateQueue.Count > 0)
                {
                    _currentState = _stateQueue.Dequeue();
                }
                _currentState?.OnEnter();
                return;
            }
            
            _currentState.OnUpdate(dt);
            
            // State가 2개 이상일 경우, 현재 State를 제외한 나머지 State를 모두 제거
            while (_stateQueue.Count > 1)
            {
                _currentState = _stateQueue.Dequeue();
            }

            if (_stateQueue.Count > 0)
            {
                _currentState.OnExit();
                _currentState = _stateQueue.Dequeue();
                _currentState.OnEnter();
            }
        }
        
        #endregion
    }
}
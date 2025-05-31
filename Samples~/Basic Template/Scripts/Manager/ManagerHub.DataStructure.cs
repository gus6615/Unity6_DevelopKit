using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace DevelopKit.BasicTemplate
{
    [Serializable]
    public class SceneData
    {
        [SerializeField] private string sceneName;
        public string SceneName => sceneName;
        
        [SerializeField] private string addressableName;
        public string AddressableName => addressableName;
    }
    
    /// <summary>
    /// [주의] 값이 클수록 초기화(Initialize) 순서가 빠름
    /// </summary> 
    public enum ManagerPriority
    {
        Atlas,
        Save,
        Scene,
        Sound,
        UI
    }
    
    public abstract class Manager : MonoBehaviour
    {
        public virtual bool IsReady { get; set; }
        public abstract ManagerPriority Priority { get; }
        public abstract UniTask StartUp();
    }
    
    // * 씬 전환 시 비동기 장면 전환을 위한 Wrapper 클래스
    public class SceneLoadAsyncOperationWrapper
    {
        public AsyncOperationHandle<SceneInstance>? asyncOperationHandle;
        public event Action Completed;
        public float Progress => asyncOperationHandle?.PercentComplete ?? 0f;
        public bool IsDone => asyncOperationHandle?.IsDone ?? false;
        
        internal void SetAsyncOperation(AsyncOperationHandle<SceneInstance> asyncOperationHandle)
        {
            this.asyncOperationHandle = asyncOperationHandle;
            asyncOperationHandle.Completed += CompleteCallback;
        }
        
        private void CompleteCallback(AsyncOperationHandle<SceneInstance> operation)
        {
            Completed?.Invoke();
            Completed = null;
        }
    }

    public sealed partial class ManagerHub
    {
        private static readonly Dictionary<string, Manager> ManagerDict = new();

        private Manager CreateManager(Type type, GameObject parent)
        {
            GameObject go = new GameObject(type.Name);
            go.transform.SetParent(parent.transform);
            return go.AddComponent(type) as Manager;
        }
    }
}
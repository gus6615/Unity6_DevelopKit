using System;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public abstract class UIBase : CachedMonobehaviour
    {
        [SerializeField] private Animator baseAnimator;
        public Animator BaseAnimator => baseAnimator;
        
        private event Action<UIBase> OnStartEnterCallback;
        private event Action<UIBase> OnEndEnterCallback;
        private event Action<UIBase> OnStartExitCallback;
        private event Action<UIBase> OnEndExitCallback;
        
        private string enterClipName;
        private string exitClipName;
        
        public bool HasEnterAnimation { get; private set; }
        public bool HasExitAnimation { get; private set; }
        
        protected virtual void Awake()
        {
            if (baseAnimator == null) return;
            
            foreach (var clip in baseAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name.Contains("Enter"))
                {
                    HasEnterAnimation = true;
                    enterClipName = clip.name;
                    clip.AddEvent(CreateAnimationEvent(0, nameof(StartEnterFunction), enterClipName));
                    clip.AddEvent(CreateAnimationEvent(clip.length, nameof(EndEnterFunction), enterClipName));
                }
                else if (clip.name.Contains("Exit"))
                {
                    HasExitAnimation = true;
                    exitClipName = clip.name;
                    clip.AddEvent(CreateAnimationEvent(0, nameof(StartExitFunction), exitClipName));
                    clip.AddEvent(CreateAnimationEvent(clip.length, nameof(EndExitFunction), exitClipName));
                }
            }
        }

        public virtual void OnEnter(object data = null)
        {
            this.transform.localScale = Vector3.zero;
        }

        public virtual void OnExit()
        {
            
        }
        
        public void StartEnterAnimation(Action<UIBase> startCallback = null, Action<UIBase> endCallback = null)
        {
            OnStartEnterCallback = startCallback;
            OnEndEnterCallback = endCallback;
            
            if (HasEnterAnimation)
            {
                baseAnimator.Play(enterClipName, 0, 0);
            }
            else
            {
                StartEnterFunction();
                EndEnterFunction();
            }
        }
        
        public void StartExitAnimation(Action<UIBase> startCallback = null, Action<UIBase> endCallback = null)
        {
            OnStartExitCallback = startCallback;
            OnEndExitCallback = endCallback;
            
            if (HasExitAnimation)
            {
                baseAnimator.Play(exitClipName, 0, 0);
            }
            else
            {
                StartExitFunction();
                EndExitFunction();
            }
        }
        
        private AnimationEvent CreateAnimationEvent(float time, string functionName, string stringParameter)
        {
            var animationEvent = new AnimationEvent
            {
                time = time,
                functionName = functionName,
                stringParameter = stringParameter
            };
            return animationEvent;
        }
        
        private void StartEnterFunction() => OnStartEnterCallback?.Invoke(this);
        private void EndEnterFunction() => OnEndEnterCallback?.Invoke(this);
        private void StartExitFunction() => OnStartExitCallback?.Invoke(this);
        private void EndExitFunction() => OnEndExitCallback?.Invoke(this);
    }
}

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    public sealed class SceneProgress_RotateCircle : SceneProgress_Base
    {

        private SceneLoadAsyncOperationWrapper wrapper;

        public static SceneProgress_RotateCircle CreateInstance()
            => AddressableUtil.Instantiate<SceneProgress_RotateCircle>("UI/SceneProgress_RotateCircle");
        
        public override void Initialize(object data = null)
        {
            base.Initialize(data);
            
            wrapper = data as SceneLoadAsyncOperationWrapper;
            if (wrapper == null)
            {
                Debug.LogError("SceneLoadAsyncOperationWrapper가 아닙니다.");
                return;
            }

            wrapper.Completed += OnComplete;
            OnProgressAsync().Forget();
        }
        
        public override async UniTask OnProgressAsync()
        {
            if (wrapper == null)
            {
                Debug.LogError("SceneLoadAsyncOperationWrapper가 없습니다.");
                return;
            }
            
            while (!wrapper.IsDone)
            {
                // float progress = Mathf.Clamp01(wrapper.Progress / 0.9f);
                await UniTask.Yield();
            }
        }
        
        public override void OnComplete()
        {
            if (wrapper == null)
            {
                Debug.LogError("SceneLoadAsyncOperationWrapper가 없습니다.");
                return;
            }
            
            AddressableUtil.Release(this.gameObject);
        }
    }
}
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    public interface IProgress
    {
        void Initialize(object data = null);
        UniTask OnProgressAsync();
        void OnComplete();
    }

    public abstract class SceneProgress_Base : MonoBehaviour, IProgress
    {
        public virtual void Initialize(object data = null) => gameObject.SetActive(true);
        public virtual async UniTask OnProgressAsync() => await UniTask.Yield();
        public abstract void OnComplete();
    }
    
    public sealed class SceneProgress_Default : SceneProgress_Base
    {
        public override void OnComplete() { }
    }
}
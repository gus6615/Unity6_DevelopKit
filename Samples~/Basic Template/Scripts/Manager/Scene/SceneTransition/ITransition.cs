using UnityEngine;
using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    public interface ITransition
    {
        UniTask FadeInAsync();
        UniTask FadeOutAsync(bool withDelete);
    }
    
    public abstract class SceneTransitionFX_Base : MonoBehaviour, ITransition
    {
        public abstract Canvas Canvas { get; }
        public abstract UniTask FadeInAsync();
        public abstract UniTask FadeOutAsync(bool withDelete);
    }
    
    public sealed class SceneTransitionFX_Default : SceneTransitionFX_Base
    {
        public override Canvas Canvas => null;
        public override async UniTask FadeInAsync() => await UniTask.Yield();
        public override async UniTask FadeOutAsync(bool withDelete) => await UniTask.Yield();
    }
}
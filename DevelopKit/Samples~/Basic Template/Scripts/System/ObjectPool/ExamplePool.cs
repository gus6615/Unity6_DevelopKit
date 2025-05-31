using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    public class ExamplePool : Singleton<ExamplePool>, IStartUpTask
    {
        private readonly string _prefabKey = "ExampleGO";
        private GameObjectPool<ExampleGameObject> _pool;
        
        public bool IsDone { get; set; }
        public StartUpPriority Priority => StartUpPriority.Default;
        public UniTask StartUp()
        {
            Instance._pool = new GameObjectPool<ExampleGameObject>();
            Instance._pool.Bind(_prefabKey, ManagerHub.UI.MainCanvas.transform);
            SceneManager.OnSceneLoadedDone += ReBind;
            return UniTask.CompletedTask;
        }
        
        public static ExampleGameObject Get()
        {
            return Instance._pool.Get();
        }
        
        public static void Release(ExampleGameObject obj)
        {
            Instance._pool.Release(obj);
        }
        
        private async UniTask ReBind(object nextSceneData)
        {
            await UniTask.Yield();
 
            Instance._pool.Bind(_prefabKey, ManagerHub.UI.MainCanvas.transform);
        }
    }
}
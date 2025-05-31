using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    public sealed partial class ManagerHub : SingletonMonoBehaviour<ManagerHub>
    {
        [Header("[ 오디오 믹서의 어드레서블 키를 입력 ]")]
        [Space(10)]
        [SerializeField] private string audioMixerKey;
        public static string AudioMixerKey => Instance.audioMixerKey;
        
        [Header("[ SceneDataBase의 어드레서블 키를 입력 ]")]
        [Space(10)]
        [SerializeField] private string sceneDataBaseKey;
        public static string SceneDataBaseKey => Instance.sceneDataBaseKey;
        
        [Header(" AtlasDataBase의 어드레서블 키를 입력 ")]
        [Space(10)]
        [SerializeField] private string atlasDataBaseKey;
        public static string AtlasDataBaseKey => Instance.atlasDataBaseKey;
        
        private static T GetManager<T>() where T : Manager
        {
            if (ManagerDict.TryGetValue(typeof(T).Name, out Manager manager) == false)
            {
                Debug.LogError($"'{typeof(T).Name}'를 찾을 수 없습니다.");
                return null;
            }
            return (T)manager;
        }
        
        public static AtlasManager Atlas => GetManager<AtlasManager>();
        public static SaveManager Save => GetManager<SaveManager>();
        public static SceneManager Scene => GetManager<SceneManager>();
        public static SoundManager Sound => GetManager<SoundManager>();
        public static UIManager UI => GetManager<UIManager>();
        
        // ... 추후 새로운 Manager 추가 (OOP 원칙에 어긋나지만 편의성을 위해 ㅎㅎ...)
        
        public async UniTask Initialize()
        {
            var managers = ReflectUtil.GetAllImplementTypes<Manager>();
            foreach (var data in managers)
            {
                Manager manager = CreateManager(data, this.gameObject);
                ManagerDict.Add(data.Name, manager);
            }
            await StartUpManagers();
        }

        private async UniTask StartUpManagers()
        {
            var managers = ManagerDict.Values.ToList();
            // 우선 순위에 따라 정렬
            managers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            await UniTask.WhenAll(managers.Select(x =>
            {
                x.StartUp();
                x.IsReady = true;
                Debug.Log($"[Manager] {x.GetType().Name} is Ready");
                return UniTask.CompletedTask;   
            }));
        }
    }
}

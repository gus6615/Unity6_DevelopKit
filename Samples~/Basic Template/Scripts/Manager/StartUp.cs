using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace DevelopKit.BasicTemplate
{
    public class StartUp : MonoBehaviour
    {
        [SerializeField] private string nextSceneName;
        
        private async void Awake()
        {
            // 프로젝트 세팅
            var projectDataBase = await Addressables.LoadAssetAsync<ProjectDataBase>("ProjectDataBase");
            SetProjectData(projectDataBase);
            Addressables.Release(projectDataBase);
            
            // 매니저 허브 설정 (ex: SoundManager, AtlasManager, SceneManager, UIManager)
            ManagerHub managerHub = AddressableUtil.Instantiate<ManagerHub>("ManagerHub");
            DontDestroyOnLoad(managerHub.gameObject);
            await ManagerHub.Instance.Initialize();
            
            await StartUpTasks();

            GameFlowManager gameFlowManager = GameFlowManager.Instance;
            gameFlowManager.Initialize();
            
            // 타이틀 씬으로 이동
            var transition = SceneTransitionFX_FadeInOut.CreateInstance();
            var progress = SceneProgress_RotateCircle.CreateInstance();
            ManagerHub.Scene.GoToNextSceneWithLoading(nextSceneName, null, transition, progress).Forget();    
        }
        
        private async UniTask StartUpTasks()
        {
            var tasks = ReflectUtil.GetAllImplementTypes<IStartUpTask>();
            List<IStartUpTask> startUpTasks = new();

            foreach (var task in tasks)
            {
                var startUpTask = Activator.CreateInstance(task) as IStartUpTask;
                if (startUpTask == null || startUpTask.IsDone)
                {
                    continue;
                }
                startUpTasks.Add(startUpTask);
            }

            startUpTasks.Sort((x, y) => y.Priority.CompareTo(x.Priority));
            foreach (var task in startUpTasks)
            {
                Debug.Log($"[StartUp] {task.GetType().Name}");
                await task.StartUp();
                task.IsDone = true;
            }
        }
        
        private void SetProjectData(ProjectDataBase projectDataBase)
        {
            QualitySettings.vSyncCount = projectDataBase.VSyncCount;
            Application.targetFrameRate = projectDataBase.TargetFrameRate;
            Screen.sleepTimeout = projectDataBase.SleepTimeout; ;
            Screen.SetResolution(projectDataBase.ResolutionWidth, projectDataBase.ResolutionHeight, FullScreenMode.FullScreenWindow);
        }
    }
}

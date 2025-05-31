using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace DevelopKit.BasicTemplate
{
    public sealed class SceneManager : Manager
    {
        public bool IsChangingScene { get; private set; }
        private readonly Dictionary<string, SceneData> sceneDataDic = new();
        public override ManagerPriority Priority => ManagerPriority.Scene;
        
        public delegate UniTask SceneLoadedDoneTask(object sceneData);
        private static readonly List<SceneLoadedDoneTask> LoadedDoneTasks = new();
        public static event SceneLoadedDoneTask OnSceneLoadedDone
        {
            add => LoadedDoneTasks.Add(value);
            remove => LoadedDoneTasks.Remove(value);
        }
        
        public override async UniTask StartUp()
        {
            var sceneDatabaseKey = ManagerHub.SceneDataBaseKey;
            
            var sceneDatabase = await Addressables.LoadAssetAsync<SceneDataBase>(sceneDatabaseKey);
            if (sceneDatabase == null)
            {
                Debug.LogError("SceneDataBase이 없습니다.");
                await UniTask.CompletedTask;
            }
            
            sceneDataDic.Clear();
            foreach (var sceneData in sceneDatabase.SceneDatas)
                sceneDataDic.Add(sceneData.SceneName, sceneData);
            
            Addressables.Release(sceneDatabase);
        }

        // * 무거운 씬 전환 (로딩 씬 추가)
        public async UniTask GoToNextSceneWithLoading(string nextScene, object nextSceneData = null, SceneTransitionFX_Base transition = null, SceneProgress_Base progress = null)
        {
            if (transition == null)
            {
                transition = new GameObject().AddComponent<SceneTransitionFX_Default>();
            }
            if (progress == null)
            {
                progress = new GameObject().AddComponent<SceneProgress_Default>();
            }

            progress.transform.SetParent(transition.Canvas.transform, false);
            progress.gameObject.SetActive(false);
            
            SceneLoader.CurrentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            SceneLoader.NextSceneName = nextScene;
            SceneLoader.NextSceneData = nextSceneData;
            SceneLoader.Transition = transition;
            SceneLoader.Progress = progress;
            
            await transition.FadeInAsync();
            
            ChangeScene("SceneLoading");
        }
        
        // * 가벼운 씬 전환 (로딩 씬 없음)
        public async UniTask GoToNextScene(string nextScene, object nextSceneData = null, SceneTransitionFX_Base transition = null)
        {
            if (transition == null)
            {
                transition = new GameObject().AddComponent<SceneTransitionFX_Default>();
            }
            
            SceneLoader.CurrentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            SceneLoader.NextSceneName = nextScene;
            SceneLoader.NextSceneData = nextSceneData;
            SceneLoader.Transition = transition;
            
            var handleOperation = SceneLoader.HandleLoadSceneStartTasks();
            await transition.FadeInAsync();
            await handleOperation;
            
            ChangeScene(nextScene, nextSceneData, transition);
        }
        
        public SceneLoadAsyncOperationWrapper ChangeScene(string nextScene, object nextSceneData = null, SceneTransitionFX_Base transition = null)
        {
            var operationWrapper = new SceneLoadAsyncOperationWrapper();
            if (transition == null)
            {
                transition = new GameObject().AddComponent<SceneTransitionFX_Default>();
            }
            IsChangingScene = true;
            ChangeSceneAsync(nextScene, nextSceneData, operationWrapper, transition).AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
            return operationWrapper;
        }
        
        private async UniTask ChangeSceneAsync(string sceneName, object nextSceneData, SceneLoadAsyncOperationWrapper operationWrapper, SceneTransitionFX_Base transition)
        {
            // 1. 씬 로드 (비동기 시작)
            var asyncOperationHandler = Addressables.LoadSceneAsync(sceneName, activateOnLoad: false);
            operationWrapper.SetAsyncOperation(asyncOperationHandler);
            
            // 2. 페이드 인 처리 (현재 씬 화면 가리기)
            await transition.FadeInAsync();
            
            // 3. UI 풀 초기화
            ManagerHub.UI.ClearUIPool();
            
            if (ManagerHub.UI.IsLoadingUI)
            {
                // UI가 (Addressable) 로드 중이라면 다음 씬에서 UI를 로드하지 않는 작업 추가 구현
            }
            
            // 4. 씬 로드 완료까지 대기
            var nextSceneInstance = await asyncOperationHandler;
            
            // 5. 씬 전환 완료
            await nextSceneInstance.ActivateAsync();
            
            ManagerHub.UI.ResetRefs();
            ManagerHub.UI.InitAllUIBase(nextSceneData);
            await UniTask.WhenAll(LoadedDoneTasks.Select(x => x.Invoke(nextSceneData)));
            
            // 6. 페이드 아웃 처리 (다음 씬 화면 보이기)
            await transition.FadeOutAsync(true);
            
            IsChangingScene = false;
        }
    }
}
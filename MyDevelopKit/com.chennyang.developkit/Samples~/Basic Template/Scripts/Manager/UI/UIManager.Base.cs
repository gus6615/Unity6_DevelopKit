using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace DevelopKit.BasicTemplate
{
    public sealed partial class UIManager : Manager
    {
        private Dictionary<Type, UIProperty> _uiProperties;
        private Dictionary<Type, Queue<GameObject>> _uiPool;
        private GameObject _uiPoolParent;
        
        private int _sortingOrder = 10;
        public bool IsLoadingUI;

        private Canvas _rootCanvas;
        private GameObject _mainCanvas;
        public GameObject MainCanvas => _mainCanvas;
        private GameObject _floatingCanvas;
        
        public override ManagerPriority Priority => ManagerPriority.UI;

        public override UniTask StartUp()
        {
            _uiProperties = new Dictionary<Type, UIProperty>();
            _uiPool = new Dictionary<Type, Queue<GameObject>>();
            
            _uiPoolParent = new GameObject("Pool");
            _uiPoolParent.transform.SetParent(ManagerHub.UI.transform, false);
            AddCanvas(_uiPoolParent, false);
            
            var uiAllTypes = ReflectUtil.GetAllImplementTypes<UIBase>();
            foreach (var uiType in uiAllTypes)
            {
                var property = ReflectUtil.GetAttribute<UIPropertyAttribute>(uiType);
                if (property == null)
                {
                    Debug.LogError($"{uiType.Name}에 UIPropertyAttribute가 없습니다.");
                    continue;
                }
                
                _uiProperties.Add(uiType, new UIProperty(property.UIType, property.AddressableKey));
            }
            
            ResetRefs();
            return UniTask.CompletedTask;
        }

        public Canvas AddCanvas(GameObject target, bool hasRayCaster = true)
        {
            if (!target.TryGetComponent(out Canvas canvas))
            {
                canvas = target.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = _sortingOrder++;
            }

            if (!target.TryGetComponent(out CanvasScaler canvasScaler))
            {
                canvasScaler = target.AddComponent<CanvasScaler>();
                canvasScaler.referenceResolution = new Vector2(Resolution.width, Resolution.height);
                canvasScaler.matchWidthOrHeight = IsPortrait ? 0 : 1;
            }

            if (hasRayCaster && !target.GetComponent<GraphicRaycaster>())
                target.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        public void ResetRefs()
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            GameObject[] roots = scene.GetRootGameObjects();
            
            foreach (var node in roots)
            {
                if (node.name.Contains("RootCanvas"))
                {
                    _rootCanvas = node.GetComponent<Canvas>();
                    break;
                }
                if (node.name.Contains("MainCanvas"))
                {
                    _mainCanvas = node;
                    continue;
                }
                if (node.name.Contains("FloatingCanvas"))
                {
                    _floatingCanvas = node;
                }
            }

            if (_rootCanvas == null)
            {
                var rootCanvasObject = new GameObject("(Create)RootCanvas");
                _rootCanvas = AddCanvas(rootCanvasObject);
            }
            
            _mainCanvas = _rootCanvas.transform.Find("MainCanvas").gameObject;
            _floatingCanvas = _rootCanvas.transform.Find("FloatingCanvas").gameObject;
            
            if (_mainCanvas == null)
            {
                _mainCanvas = new GameObject("(Create)MainCanvas");
                _mainCanvas.transform.SetParent(_rootCanvas.transform, false);
                AddCanvas(_mainCanvas);
            }
            
            if (_floatingCanvas == null)
            {
                _floatingCanvas = new GameObject("(Create)FloatingCanvas");
                _floatingCanvas.transform.SetParent(_rootCanvas.transform, false);
            }
        }

        public void InitAllUIBase(object data = null)
        {
            foreach (var ui in _mainCanvas.GetComponentsInChildren<UIBase>())
            {
                ui.OnEnter(data);
                ui.StartEnterAnimation();
            }
        }
        
        private void PoolingUI(UIBase ui)
        {
            Type type = ui.GetType();
            if (_uiPool.TryGetValue(type, out Queue<GameObject> pool) == false)
            {
                pool = new Queue<GameObject>();
                _uiPool.Add(type, pool);
            }

            pool.Enqueue(ui.gameObject);
        }

        private UIBase UnPoolingUI(Type ui)
        {
            if (_uiPool.TryGetValue(ui, out Queue<GameObject> pool) == false)
            {
                Debug.LogError($"{ui.Name} 풀이 없습니다.");
                return null;
            }
            
            if (pool.Count == 0)
            {
                Debug.LogError($"{ui.Name} 풀이 비어있습니다.");
                return null;
            }
            
            GameObject uiObject = pool.Dequeue();
            return uiObject.GetComponent<UIBase>();
        }
        
        public void ClearUIPool()
        {
            foreach (var pool in _uiPool)
            {
                foreach (var ui in pool.Value)
                {
                    AddressableUtil.Release(ui.gameObject);
                    Destroy(ui);
                }
            }
            
            _uiPool.Clear();
        }
        
        private async UniTask<T> LoadUI<T>(Type uiType) where T : UIBase
        {
            UIProperty property = _uiProperties[uiType];
            GameObject uiObject = await Addressables.InstantiateAsync(property.AddressableKey, _mainCanvas.transform);
            return uiObject.GetComponent<T>();
        }
    }
}
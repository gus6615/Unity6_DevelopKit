using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    public sealed partial class UIManager
    {
        public static Resolution Resolution => Screen.currentResolution;
        public static bool IsPortrait => Screen.width < Screen.height;
        
        public async UniTask<T> ShowUIAsync<T>(object data = null, Action<UIBase> startAnimCallback = null, Action<UIBase> endAnimCallback = null) where T : UIBase
        {
            var uiType = typeof(T);
            if (_uiProperties.ContainsKey(uiType) == false)
            {
                Debug.LogError($"{uiType.Name}에 UIPropertyAttribute가 없습니다.");
                return null;
            }
            
            T ui;

            if (_uiPool.TryGetValue(uiType, out Queue<GameObject> pool) && pool.Count > 0)
            {
                // UI Pool에서 가져오기 
                ui = (T)UnPoolingUI(uiType);
            }
            else
            {
                // Addressable에서 로드
                ui = await LoadUI<T>(uiType);
            }
            
            var uiObject = ui.gameObject;
            uiObject.transform.SetParent(_mainCanvas.transform, false);
            uiObject.transform.SetAsLastSibling();
            uiObject.name = uiType.Name;
            uiObject.SetActive(true);
            
            ui.OnEnter(data);
            ui.StartEnterAnimation(startAnimCallback, endAnimCallback);
            
            return ui;
        }

        public void HideUI(UIBase ui, Action<UIBase> startCallback = null, Action<UIBase> endCallback = null)
        {
            PoolingUI(ui);
            
            ui.OnExit();
            ui.StartExitAnimation(startCallback, (_) =>
            {
                endCallback?.Invoke(ui);
                ui.gameObject.SetActive(false);
                ui.transform.SetParent(_uiPoolParent.transform, false);
            });
        }
    }
}
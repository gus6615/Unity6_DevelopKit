using System;
using UnityEngine;
using UnityEngine.UI;

namespace DevelopKit.BasicTemplate
{
    [UIProperty(UIType.Alert, "UI/ConfirmAlert")]
    public class ConfirmAlert : UIBase
    {
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        
        private Action _onYesAction;
        private Action _onNoAction;

        protected override void Awake()
        {
            base.Awake();
            yesButton.onClick.AddListener(OnClickYes);
            noButton.onClick.AddListener(OnClickNo);
        }
        
        private void OnDestroy()
        {
            yesButton.onClick.RemoveListener(OnClickYes);
            noButton.onClick.RemoveListener(OnClickNo);
        }

        public override void OnEnter(object data = null)
        {
            base.OnEnter(data);
            
            CallbackTuple callbackTuple = data as CallbackTuple;
            if (callbackTuple == null) return;
            
            _onYesAction = callbackTuple.YesAction;
            _onNoAction = callbackTuple.NoAction;
        }
        
        private void OnClickYes()
        {
            ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
            _onYesAction?.Invoke();
            ManagerHub.UI.HideUI(this);
        }
        
        private void OnClickNo()
        {
            ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
            _onNoAction?.Invoke();
            ManagerHub.UI.HideUI(this);
        }
    }
}
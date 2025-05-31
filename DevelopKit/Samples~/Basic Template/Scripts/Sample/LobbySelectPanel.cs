using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    [UIProperty(UIType.Overlay, "UI/LobbySelectPanel")]
    public class LobbySelectPanel : UIBase
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button closeButton;
        
        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(OnClickContinue);
            newGameButton.onClick.AddListener(OnClickNewGame);
            settingButton.onClick.AddListener(OnClickSetting);
            closeButton.onClick.AddListener(OnClickExit);
        }

        private void OnDestroy()
        {
            continueButton.onClick.RemoveListener(OnClickContinue);
            newGameButton.onClick.RemoveListener(OnClickNewGame);
            settingButton.onClick.RemoveListener(OnClickSetting);
            closeButton.onClick.RemoveListener(OnClickExit);
        }

        public override void OnEnter(object data = null)
        {
            base.OnEnter(data);
            
            ManagerHub.Sound.PlayBGM(BGM.BGM_0);
            GameFlowManager.Instance.PushState<LobbyState>();
            
            var isUserDataExist = ManagerHub.Save.FindUserData();
            continueButton.gameObject.SetActive(isUserDataExist);
        }

        public void PlayHoverSound() => ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
        
        private void OnClickContinue()
        {
            ManagerHub.Sound.PlaySFX(SFX.SFX_0);
            
            var transition = SceneTransitionFX_FadeInOut.CreateInstance();
            ManagerHub.Scene.GoToNextScene("InGameScene", null, transition).Forget();    
        }
        
        private void OnClickNewGame()
        {
            ManagerHub.Sound.PlaySFX(SFX.SFX_0);

            if (ManagerHub.Save.FindUserData())
            {
                CallbackTuple callbackTuple = new CallbackTuple(OnClickNewGameYes, null);
                ManagerHub.UI.ShowUIAsync<ConfirmAlert>(callbackTuple).Forget();
            }
            else
            {
                ManagerHub.Save.ResetData();

                var transition = SceneTransitionFX_FadeInOut.CreateInstance();
                ManagerHub.Scene.GoToNextScene("InGameScene", null, transition).Forget();    
            }
        }
        
        private void OnClickSetting()
        {
            ManagerHub.Sound.PlaySFX(SFX.SFX_0);
            ManagerHub.UI.ShowUIAsync<SettingPanel>().Forget();
        }
        
        private void OnClickExit()
        {
            ManagerHub.Sound.PlaySFX(SFX.SFX_0);
#if UNITY_EDITOR
            GameLifeManager.InvokeQuitCallback();
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // 어플리케이션 종료
#endif
        }

        private void OnClickNewGameYes()
        {
            ManagerHub.Save.ResetData();
            
            var transition = SceneTransitionFX_FadeInOut.CreateInstance();
            ManagerHub.Scene.GoToNextScene("InGameScene", null, transition).Forget();    
        }
    }
}
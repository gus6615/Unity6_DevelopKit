using UnityEngine;
using UnityEngine.UI;

namespace DevelopKit.BasicTemplate
{
    [UIProperty(UIType.Overlay, "UI/SettingPanel")]
    public class SettingPanel : UIBase
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        
        [SerializeField] private Button masterOnOffButton;
        [SerializeField] private Button bgmOnOffButton;
        [SerializeField] private Button sfxOnOffButton;
        [SerializeField] private Button closeButton;
        
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;
        
        protected override void Awake()
        {
            base.Awake();

            masterSlider.onValueChanged.AddListener(OnValueChangedMasterVolume);
            bgmSlider.onValueChanged.AddListener(OnValueChangedBGMVolume);
            sfxSlider.onValueChanged.AddListener(OnValueChangedSFXVolume);
            
            masterOnOffButton.onClick.AddListener(OnClickMasterToggle);
            bgmOnOffButton.onClick.AddListener(OnClickBGMToggle);
            sfxOnOffButton.onClick.AddListener(OnClickSFXToggle);
            
            closeButton.onClick.AddListener(OnClickClose);
        }
        
        private void OnDestroy()
        {
            masterSlider.onValueChanged.RemoveListener(OnValueChangedMasterVolume);
            bgmSlider.onValueChanged.RemoveListener(OnValueChangedBGMVolume);
            sfxSlider.onValueChanged.RemoveListener(OnValueChangedSFXVolume);
            
            masterOnOffButton.onClick.RemoveListener(OnClickMasterToggle);
            bgmOnOffButton.onClick.RemoveListener(OnClickBGMToggle);
            sfxOnOffButton.onClick.RemoveListener(OnClickSFXToggle);
            
            closeButton.onClick.RemoveListener(OnClickClose);
        }

        public override void OnEnter(object data = null)
        {
            base.OnEnter(data);

            masterOnOffButton.image.sprite = ManagerHub.Sound.MasterVolume.Value > 0.0f ? onSprite : offSprite;
            bgmOnOffButton.image.sprite = ManagerHub.Sound.BGMVolume.Value > 0.0f ? onSprite : offSprite;
            sfxOnOffButton.image.sprite = ManagerHub.Sound.SFXVolume.Value > 0.0f ? onSprite : offSprite;
            
            masterSlider.value = ManagerHub.Sound.IsOnMaster.Value ? ManagerHub.Sound.MasterVolume.Value : 0.0f;
            bgmSlider.value = ManagerHub.Sound.IsOnBGM.Value ? ManagerHub.Sound.BGMVolume.Value : 0.0f;
            sfxSlider.value = ManagerHub.Sound.IsOnSFX.Value ? ManagerHub.Sound.SFXVolume.Value : 0.0f;
        }

        private void OnClickClose()
        {
            ManagerHub.UI.HideUI(this);
        }
        
        private void OnValueChangedMasterVolume(float volume)
        {
            if (volume > 0.0f && !ManagerHub.Sound.IsOnMaster.Value)
            {
                ManagerHub.Sound.EnableMasterChannel(true);
            }
            ManagerHub.Sound.SetMasterVolume(volume);
            masterOnOffButton.image.sprite = volume > 0.0f ? onSprite : offSprite;
        }
        
        private void OnValueChangedBGMVolume(float volume)
        {
            if (volume > 0.0f && !ManagerHub.Sound.IsOnBGM.Value)
            {
                ManagerHub.Sound.EnableBGMChannel(true);
            }
            ManagerHub.Sound.SetBGMVolume(volume);
            bgmOnOffButton.image.sprite = volume > 0.0f ? onSprite : offSprite;
        }
        
        private void OnValueChangedSFXVolume(float volume)
        {
            if (volume > 0.0f && !ManagerHub.Sound.IsOnSFX.Value)
            {
                ManagerHub.Sound.EnableSFXChannel(true);
            }
            ManagerHub.Sound.SetSFXVolume(volume);
            sfxOnOffButton.image.sprite = volume > 0.0f ? onSprite : offSprite;
        }

        private void OnClickMasterToggle()
        {
            ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
            
            bool isOn = ManagerHub.Sound.IsOnMaster.Value ? false : true;
            ManagerHub.Sound.EnableMasterChannel(isOn);
            masterOnOffButton.image.sprite = isOn ? onSprite : offSprite;
            masterSlider.SetValueWithoutNotify(isOn ? ManagerHub.Sound.MasterVolume.Value : 0.0f);
        }
        
        private void OnClickBGMToggle()
        {
            ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
            
            bool isOn = ManagerHub.Sound.IsOnBGM.Value ? false : true;
            ManagerHub.Sound.EnableBGMChannel(isOn);
            bgmOnOffButton.image.sprite = isOn ? onSprite : offSprite;
            bgmSlider.SetValueWithoutNotify(isOn ? ManagerHub.Sound.BGMVolume.Value : 0.0f);
        }
        
        private void OnClickSFXToggle()
        {
            ManagerHub.Sound.PlaySFX(SFX.Click_Mechanical);
            
            bool isOn = ManagerHub.Sound.IsOnSFX.Value ? false : true;
            ManagerHub.Sound.EnableSFXChannel(isOn);
            sfxOnOffButton.image.sprite = isOn ? onSprite : offSprite;
            sfxSlider.SetValueWithoutNotify(isOn ? ManagerHub.Sound.SFXVolume.Value : 0.0f);
        }
    }
}
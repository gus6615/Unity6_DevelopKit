using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public partial class SoundManager
    {
        public void PlayBGM(BGM bgm)
        {
            AudioClip audioClip = GetOrAddAudioClip($"BGM/{bgm}");
            Play(audioClip, SoundType.BGM);
        }
        
        public void PlaySFX(SFX sfx)
        {
            AudioClip audioClip = GetOrAddAudioClip($"SFX/{sfx}");
            Play(audioClip, SoundType.SFX);
        }

        public void EnableMasterChannel(bool isEnabled)
        {
            IsOnMaster.Value = isEnabled;
            
            float volume = isEnabled ? MasterVolume.Value : 0.0f;
            ApplyVolumeToAudioMixer(MasterVolumeParam, volume);
        }
        
        public void EnableBGMChannel(bool isEnabled)
        {
            IsOnBGM.Value = isEnabled;
            
            float volume = isEnabled ? BGMVolume.Value : 0.0f;
            ApplyVolumeToAudioMixer(BGMVolumeParam, volume);
        }
        
        public void EnableSFXChannel(bool isEnabled)
        {
            IsOnSFX.Value = isEnabled;
            
            float volume = isEnabled ? SFXVolume.Value : 0.0f;
            ApplyVolumeToAudioMixer(SFXVolumeParam, volume);
        }

        public void SetMasterVolume(float volume, bool shouldSave = true)
        {
            ApplyVolumeToAudioMixer(MasterVolumeParam, volume);
            
            if (shouldSave)
            {
                MasterVolume.Value = volume;
            }
        }

        public void SetBGMVolume(float volume, bool shouldSave = true)
        {
            ApplyVolumeToAudioMixer(BGMVolumeParam, volume);
            
            if (shouldSave)
            {
                BGMVolume.Value = volume;
            }
        }

        public void SetSFXVolume(float volume, bool shouldSave = true)
        {
            ApplyVolumeToAudioMixer(SFXVolumeParam, volume);
            
            if (shouldSave)
            {
                SFXVolume.Value = volume;
            }
        }
    }
}
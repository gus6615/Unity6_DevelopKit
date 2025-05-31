using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    public partial class SoundManager : Manager
    {
        private const string MasterVolumeParam = "MasterVolume";
        private const string BGMVolumeParam = "BGMVolume";
        private const string SFXVolumeParam = "SFXVolume";
        
        private AudioMixer audioMixer;
        
        private Dictionary<string, AudioClip> _audioClipDic;
        private AudioSource[] _audioSources;

        public ObservableProperty<bool> IsOnMaster { get; private set; }
        public ObservableProperty<bool> IsOnBGM { get; private set; }
        public ObservableProperty<bool> IsOnSFX { get; private set; }
        public ObservableProperty<float> MasterVolume { get; private set; }
        public ObservableProperty<float> BGMVolume { get; private set; }
        public ObservableProperty<float> SFXVolume { get; private set; }
        
        public override ManagerPriority Priority => ManagerPriority.Sound;
        
        public override async UniTask StartUp()
        {
            string audioMixerKey = ManagerHub.AudioMixerKey;
            
            if (string.IsNullOrEmpty(audioMixerKey))
            {
                Debug.LogError("AudioMixer Key is null or empty");
                await UniTask.CompletedTask;
            }
            if (!AddressableUtil.IsValidKey(audioMixerKey))
            {
                Debug.LogError($"Invalid Key : {audioMixerKey}");
                await UniTask.CompletedTask;
            }
            
            audioMixer = AddressableUtil.LoadAsset<AudioMixer>(audioMixerKey);
            
            string[] soundTypeNames = Enum.GetNames(typeof(SoundType));
            _audioClipDic = new Dictionary<string, AudioClip>();
            _audioSources = new AudioSource[(int)SoundType.CNT];
            
            for (int i = 0; i < _audioSources.Length; i++)
            {
                GameObject go = new GameObject(soundTypeNames[i]);
                _audioSources[i] = go.AddComponent<AudioSource>();
                _audioSources[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups(soundTypeNames[i])[0];
                go.transform.SetParent(ManagerHub.Sound.transform);
            }

            IsOnMaster = new ObservableProperty<bool>(PlayerPrefs.GetInt("IsOnMaster", 1) == 1);
            IsOnBGM = new ObservableProperty<bool>(PlayerPrefs.GetInt("IsOnBGM", 1) == 1);
            IsOnSFX = new ObservableProperty<bool>(PlayerPrefs.GetInt("IsOnSFX", 1) == 1);
            
            MasterVolume = new ObservableProperty<float>(PlayerPrefs.GetFloat("MasterVolume", 1.0f));
            BGMVolume = new ObservableProperty<float>(PlayerPrefs.GetFloat("BGMVolume", 1.0f));
            SFXVolume = new ObservableProperty<float>(PlayerPrefs.GetFloat("SFXVolume", 1.0f));
                
            EnableMasterChannel(IsOnMaster.Value);
            EnableBGMChannel(IsOnBGM.Value);
            EnableSFXChannel(IsOnSFX.Value);
            
            GameLifeManager.OnQuitedCallback += SaveSoundSetting;
        }
        
        private void Play(AudioClip audioClip, SoundType soundType)
        {
            if (audioClip == null)
            {
                Debug.Log("Failed to Play : AudioClip is null");
                return;
            }

            AudioSource audioSource = _audioSources[(int)soundType];

            switch (soundType)
            {
                case SoundType.BGM:
                    if (audioSource.isPlaying)
                        audioSource.Stop();

                    audioSource.clip = audioClip;
                    audioSource.loop = true;
                    audioSource.Play();
                    break;

                case SoundType.SFX:
                    audioSource.PlayOneShot(audioClip);
                    break;
            }
        }

        private float Volume2Db(float volume) => Mathf.Log(Mathf.Max(volume, 0.0001f)) * 20;
        private void ApplyVolumeToAudioMixer(string param, float volume) => audioMixer.SetFloat(param, Volume2Db(volume));

        private AudioClip GetOrAddAudioClip(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.Log("Failed to GetOrAddAudioClip : key is null or empty");
                return null;
            }

            if (_audioClipDic.TryGetValue(key, out var audioClip) == false)
            {
                audioClip = AddressableUtil.LoadAsset<AudioClip>(key);
                if (audioClip == null)
                {
                    Debug.Log($"Failed to GetOrAddAudioClip : {key}");
                    return null;
                }
                _audioClipDic.Add(key, audioClip);
            }

            return audioClip;
        }

        private void SaveSoundSetting()
        {
            PlayerPrefs.SetInt("IsOnMaster", IsOnMaster.Value ? 1 : 0);
            PlayerPrefs.SetInt("IsOnBGM", IsOnBGM.Value ? 1 : 0);
            PlayerPrefs.SetInt("IsOnSFX", IsOnSFX.Value ? 1 : 0);
            
            PlayerPrefs.SetFloat("MasterVolume", MasterVolume.Value);
            PlayerPrefs.SetFloat("BGMVolume", BGMVolume.Value);
            PlayerPrefs.SetFloat("SFXVolume", SFXVolume.Value);
        }
    }
}
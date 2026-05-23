using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundEntry
{
    public string soundKey;
    public AudioClip audioClip;
}

public class Knife_Audio : MonoBehaviour
{
    [Header("音效列表")]
    [Tooltip("通过关键字管理多个挥刀音效")]
    public List<SoundEntry> swingSounds = new List<SoundEntry>();

    [Header("音频源")]
    [Tooltip("专属挥刀音效音频源（需手动分配）")]
    public AudioSource knifeAudioSource;

    private AudioSource audioSource;
    private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();
    private float originalVolume = 1f;

    void Awake()
    {
        audioSource = knifeAudioSource;
        BuildSoundDictionary();
        
        if (audioSource != null)
        {
            originalVolume = audioSource.volume;
        }
    }
    
    void OnEnable()
    {
        AudioManager.OnSFXVolumeChanged += OnSFXVolumeChanged;
        // 初始化时同步一次音量
        if (AudioManager.instance != null)
        {
            float currentVolume = PlayerPrefs.GetFloat("SFXVolume", 100f) / 100f;
            UpdateVolume(currentVolume);
        }
    }
    
    void OnDisable()
    {
        AudioManager.OnSFXVolumeChanged -= OnSFXVolumeChanged;
    }
    
    private void OnSFXVolumeChanged(float volume)
    {
        UpdateVolume(volume);
    }
    
    private void UpdateVolume(float sfxVolume)
    {
        if (audioSource != null)
        {
            audioSource.volume = originalVolume * sfxVolume;
        }
    }
    
    void BuildSoundDictionary()
    {
        soundDictionary.Clear();
        foreach (SoundEntry entry in swingSounds)
        {
            if (!string.IsNullOrEmpty(entry.soundKey) && entry.audioClip != null)
            {
                if (soundDictionary.ContainsKey(entry.soundKey))
                {
                    Debug.LogWarning("重复的音效关键字: " + entry.soundKey);
                }
                else
                {
                    soundDictionary[entry.soundKey] = entry.audioClip;
                }
            }
        }
    }

    public void PlaySwingSound(string soundKey)
    {
        if (string.IsNullOrEmpty(soundKey))
        {
            Debug.LogWarning("音效关键字为空");
            return;
        }

        if (soundDictionary.TryGetValue(soundKey, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("未找到音效: " + soundKey);
        }
    }
}

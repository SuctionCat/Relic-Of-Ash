using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioType[] AudioTypes;
    /// <summary>
    /// 音频管理器实例，用于在其他脚本中调用音频管理器的方法
    /// </summary>
    public static AudioManager instance;
    private AudioSource clickSource;
    
    /// <summary>
    /// 音效音量变化事件，供其他脚本订阅
    /// </summary>
    public static event System.Action<float> OnSFXVolumeChanged;
    
    /// <summary>
    /// 音乐音量变化事件，供其他脚本订阅
    /// </summary>
    public static event System.Action<float> OnMusicVolumeChanged;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        
        // 初始化点击音效的AudioSource
        clickSource = gameObject.AddComponent<AudioSource>();
    }
    private void Start()
    {
       foreach(AudioType audioType in AudioTypes)
       {
           audioType.source = gameObject.AddComponent<AudioSource>();
           audioType.source.clip = audioType.clip;
           audioType.source.volume = audioType.volume;
           audioType.source.loop = audioType.loop;
           audioType.source.pitch = audioType.pitch;
           if(audioType.mixerGroup != null)
           {
               audioType.source.outputAudioMixerGroup = audioType.mixerGroup;
           }
       }
       
       // 初始化时应用保存的音效音量
       float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 100f);
       SetSFXVolume(savedSFXVolume);
       
       // 初始化时应用保存的音乐音量
       float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 100f);
       SetMusicVolume(savedMusicVolume);
    }
    public void Play(string name)
    {
        foreach(AudioType audioType in AudioTypes)
        {
            if(audioType.name == name)
            {
                audioType.source.Play();
                return;
            }
        }
        Debug.LogError($"AudioType {name} not found");
    }
    public void Pause(string name)
    {
        foreach(AudioType audioType in AudioTypes)
        {
            if(audioType.name == name)
            {
                audioType.source.Pause();
                return;
            }
        }
        Debug.LogError($"AudioType {name} not found");
    }
    public void Stop(string name)
    {
        foreach(AudioType audioType in AudioTypes)
        {
            if(audioType.name == name)
            {
                audioType.source.Stop();
                return;
            }
        }
        Debug.LogError($"AudioType {name} not found");
    }
    // 添加一个简单的方法，用于测试
    public void Test()
    {
        Debug.Log("AudioManager test method called");
    }
    // 播放点击音效
    public void PlayClickSound()
    {
        if(clickSource == null)
        {
            clickSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 加载Resources/Audio/Click音效
        AudioClip clickClip = Resources.Load<AudioClip>("Audio/Click");
        if(clickClip != null)
        {
            clickSource.clip = clickClip;
            clickSource.Play();
        }
        else
        {
            Debug.LogError("无法加载点击音效: Resources/Audio/Click");
        }
    }
    // 静态方法，方便在其他脚本中直接调用
    public static void PlayClick()
    {
        if(instance != null)
        {
            instance.PlayClickSound();
        }
        else
        {
            Debug.LogError("AudioManager实例不存在");
        }
    }
    
    // 设置音效音量（用于UI音效、点击音效等）
    public void SetSFXVolume(float volume)
    {
        float normalizedVolume = volume / 100f;
        if(clickSource != null)
        {
            clickSource.volume = normalizedVolume;
        }
        // 也可以设置其他音效类型的音量
        foreach(AudioType audioType in AudioTypes)
        {
            // 可以根据名称或其他条件来区分音效类型
            if(audioType.name.Contains("SFX") || audioType.name.Contains("Click"))
            {
                audioType.source.volume = normalizedVolume * audioType.volume;
            }
        }
        
        // 触发音效音量变化事件
        OnSFXVolumeChanged?.Invoke(normalizedVolume);
    }
    
    // 静态方法，设置音效音量
    public static void SetSFXVolumeStatic(float volume)
    {
        if(instance != null)
        {
            instance.SetSFXVolume(volume);
        }
    }
    
    // 设置音乐音量（控制所有AudioType的音频和场景中的Main BGM）
    public void SetMusicVolume(float volume)
    {
        float normalizedVolume = volume / 100f;
        Debug.Log($"[AudioManager] SetMusicVolume called with volume={volume}, normalized={normalizedVolume}");
        
        // 控制AudioTypes中的音频
        if(AudioTypes != null)
        {
            Debug.Log($"[AudioManager] AudioTypes has {AudioTypes.Length} elements");
            foreach(AudioType audioType in AudioTypes)
            {
                audioType.source.volume = normalizedVolume;
                Debug.Log($"[AudioManager] Set AudioType {audioType.name} volume to {normalizedVolume}");
            }
        }
        else
        {
            Debug.LogWarning("[AudioManager] AudioTypes is null");
        }
        
        // 控制场景中的BGM对象（支持多个场景）
        SetVolumeForBgmObject("Main BGM", normalizedVolume); // 场景1的BGM
        SetVolumeForBgmObject("BGM", normalizedVolume); // 场景2的BGM
        
        OnMusicVolumeChanged?.Invoke(normalizedVolume);
    }
    
    // 设置指定名称的BGM对象音量
    private void SetVolumeForBgmObject(string objectName, float volume)
    {
        GameObject bgmObject = GameObject.Find(objectName);
        if(bgmObject != null)
        {
            AudioSource bgmSource = bgmObject.GetComponent<AudioSource>();
            if(bgmSource != null)
            {
                bgmSource.volume = volume;
                Debug.Log($"[AudioManager] Set {objectName} volume to {volume}");
            }
        }
    }
    
    // 应用保存的音量到当前场景中的所有BGM对象
    public void ApplySavedMusicVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 100f);
        SetMusicVolume(savedVolume);
    }
    
    // 静态方法，应用保存的音量
    public static void ApplySavedMusicVolumeStatic()
    {
        if(instance != null)
        {
            instance.ApplySavedMusicVolume();
        }
    }
    
    // 静态方法，设置音乐音量
    public static void SetMusicVolumeStatic(float volume)
    {
        Debug.Log($"[AudioManager] SetMusicVolumeStatic called with volume={volume}, instance={instance}");
        if(instance != null)
        {
            instance.SetMusicVolume(volume);
        }
        else
        {
            Debug.LogError("[AudioManager] instance is null in SetMusicVolumeStatic");
        }
    }
    
    private void OnEnable()
    {
        // 监听场景加载完成事件
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        // 取消监听场景加载完成事件
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // 场景加载完成时调用
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"[AudioManager] Scene loaded: {scene.name}, applying saved music volume");
        ApplySavedMusicVolume();
    }
}
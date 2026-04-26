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
}
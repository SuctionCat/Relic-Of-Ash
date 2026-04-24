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
}

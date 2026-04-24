using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioType 
{
    public AudioSource source;
    public AudioMixerGroup mixerGroup;
    public AudioClip clip;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public string name;
    [Range(0f,1f)]
    public float volume;
    public bool loop;
        
    
}

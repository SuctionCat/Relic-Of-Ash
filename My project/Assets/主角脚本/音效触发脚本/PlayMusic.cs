using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [Header("当前对象的音乐")]
    public AudioClip musicClip;

    [Header("是否循环播放")]
    public bool loop = true;

    [Header("音乐固有音量 (0-1)")]
    [Range(0f, 1f)]
    public float nativeVolume = 1f;

    [Header("渐变时间 (秒)")]
    public float fadeDuration = 1f;

    private AudioSource audioSource;
    private static PlayMusic currentPlayingMusic = null;
    private static AudioSource globalAudioSource = null;
    private float musicVolumeMultiplier = 1f;
    private bool isFading = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = loop;
    }

    void OnEnable()
    {
        AudioManager.OnMusicVolumeChanged += OnMusicVolumeChanged;
    }

    void OnDisable()
    {
        AudioManager.OnMusicVolumeChanged -= OnMusicVolumeChanged;
    }

    void OnMusicVolumeChanged(float normalizedVolume)
    {
        musicVolumeMultiplier = normalizedVolume;
        if (audioSource != null && audioSource.isPlaying && currentPlayingMusic == this)
        {
            audioSource.volume = nativeVolume * musicVolumeMultiplier;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayThisMusic();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !loop)
        {
            StopMusic();
        }
    }

    public void PlayThisMusic()
    {
        if (musicClip == null)
        {
            Debug.LogWarning("音乐文件为空！");
            return;
        }

        bool isSameClipPlaying = currentPlayingMusic != null && 
                                 currentPlayingMusic.musicClip == musicClip && 
                                 globalAudioSource != null && 
                                 globalAudioSource.isPlaying;

        if (!isSameClipPlaying)
        {
            if (isFading)
            {
                StopAllCoroutines();
            }
            StartCoroutine(FadeInNewMusic());
        }
    }

    private IEnumerator FadeInNewMusic()
    {
        isFading = true;
        AudioSource previousAudioSource = globalAudioSource;

        if (previousAudioSource != null && previousAudioSource.isPlaying && previousAudioSource != audioSource)
        {
            yield return StartCoroutine(FadeOut(previousAudioSource));
            previousAudioSource.Stop();
        }

        audioSource.clip = musicClip;
        audioSource.loop = loop;
        audioSource.volume = 0f;
        audioSource.Play();

        yield return StartCoroutine(FadeIn(audioSource));

        currentPlayingMusic = this;
        globalAudioSource = audioSource;
        isFading = false;
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            yield return null;
        }

        source.volume = 0f;
    }

    private IEnumerator FadeIn(AudioSource source)
    {
        float targetVolume = nativeVolume * musicVolumeMultiplier;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, time / fadeDuration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying && !isFading)
        {
            StartCoroutine(FadeOutAndStop());
        }
    }

    private IEnumerator FadeOutAndStop()
    {
        isFading = true;
        yield return StartCoroutine(FadeOut(audioSource));
        audioSource.Stop();
        
        if (currentPlayingMusic == this)
        {
            currentPlayingMusic = null;
            globalAudioSource = null;
        }
        isFading = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk_Audio : MonoBehaviour
{
    [Header("动画列表")]
    [Tooltip("播放这些动画时使用跑步音效")]
    public List<AnimationClip> walkAnimations = new List<AnimationClip>();
    
    [Tooltip("播放这些动画时使用快速奔跑音效")]
    public List<AnimationClip> runAnimations = new List<AnimationClip>();

    [Header("音效")]
    [Tooltip("跑步音效")]
    public AudioClip walkSound;
    
    [Tooltip("快速奔跑音效")]
    public AudioClip runSound;

    [Header("音频源")]
    [Tooltip("专属脚步声音频源（需手动分配）")]
    public AudioSource walkAudioSource;

    private Animator _animator;
    private AudioSource _audioSource;
    private AnimationClip _currentAnimationClip;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = walkAudioSource;
        
        if (_audioSource != null)
        {
            _audioSource.loop = true;
        }
    }

    void Update()
    {
        CheckCurrentAnimation();
    }

    void CheckCurrentAnimation()
    {
        if (_animator == null) return;

        AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        bool isCurrentlyInWalkAnim = false;
        bool isCurrentlyInRunAnim = false;

        if (clipInfo.Length > 0)
        {
            AnimationClip currentClip = clipInfo[0].clip;
            
            if (currentClip != _currentAnimationClip)
            {
                _currentAnimationClip = currentClip;
            }

            isCurrentlyInWalkAnim = walkAnimations.Contains(currentClip);
            isCurrentlyInRunAnim = runAnimations.Contains(currentClip);
        }

        if (isCurrentlyInWalkAnim && walkSound != null)
        {
            if (_audioSource.clip != walkSound || !_audioSource.isPlaying)
            {
                _audioSource.clip = walkSound;
                _audioSource.loop = true;
                _audioSource.Play();
            }
        }
        else if (isCurrentlyInRunAnim && runSound != null)
        {
            if (_audioSource.clip != runSound || !_audioSource.isPlaying)
            {
                _audioSource.clip = runSound;
                _audioSource.loop = true;
                _audioSource.Play();
            }
        }
        else
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
    }
}

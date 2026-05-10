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

    private Animator _animator;
    private AudioSource _audioSource;
    private AnimationClip _currentAnimationClip;
    private bool _isInWalkAnimation = false;
    private bool _isInRunAnimation = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        _audioSource.loop = true;
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
            _isInWalkAnimation = true;
            _isInRunAnimation = false;
        }
        else if (isCurrentlyInRunAnim && runSound != null)
        {
            if (_audioSource.clip != runSound || !_audioSource.isPlaying)
            {
                _audioSource.clip = runSound;
                _audioSource.loop = true;
                _audioSource.Play();
            }
            _isInRunAnimation = true;
            _isInWalkAnimation = false;
        }
        else
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
            _isInWalkAnimation = false;
            _isInRunAnimation = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeAttack : MonoBehaviour
{
    private Animator animator;
    
    [Header("生命值")]
    [Tooltip("当前生命值（会自动同步到 StateManager）")]
    [SerializeField]
    private float health = 1000f;
    
    public float Health
    {
        get
        {
            if (StateManager.instance != null)
            {
                return StateManager.instance.currentHealth;
            }
            return health;
        }
        set
        {
            health = value;
            if (StateManager.instance != null)
            {
                StateManager.instance.currentHealth = value;
            }
            animator?.SetFloat("Health", value);
        }
    }
    
    [Header("护盾")]
    [Tooltip("当前护盾值（会自动同步到 StateManager）")]
    [SerializeField]
    private float shield = 200f;
    
    public float Shield
    {
        get
        {
            if (StateManager.instance != null)
            {
                return StateManager.instance.currentShield;
            }
            return shield;
        }
        set
        {
            shield = value;
            if (StateManager.instance != null)
            {
                StateManager.instance.currentShield = value;
            }
            animator?.SetFloat("Shield", value);
        }
    }
    
    [Header("受击图层设置")]
    public string behitLayerName = "Behit";
    private int behitLayerIndex = -1;
    
    [Header("受击动画设置")]
    public List<string> hitAnimationNames = new List<string> { "Hit_Combat_F" };
    
    [Header("基础层允许动画")]
    [Tooltip("只有当Base Layer播放以下动画时，才允许触发Behit权重变化")]
    public List<string> allowedBaseLayerAnimations = new List<string>();
    
    [Header("平滑过渡设置")]
    [Tooltip("权重从0过渡到1的时间（秒）")]
    public float fadeInTime = 0.1f;
    [Tooltip("权重从1过渡到0的时间（秒）")]
    public float fadeOutTime = 0.2f;
    
    [Header("特效管理")]
    [Tooltip("用于管理攻击特效的ComboEffectManager组件")]
    public ComboEffectManager comboEffectManager;
    [Tooltip("受击时需要停止的攻击特效Key列表")]
    public List<string> attackEffectKeys = new List<string>();
    
    private bool isPlayingHitAnimation = false;
    private bool isTransitioning = false;
    private bool isDead = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            behitLayerIndex = animator.GetLayerIndex(behitLayerName);
            if (behitLayerIndex == -1)
            {
                Debug.LogWarning($"未找到名为 {behitLayerName} 的动画图层");
            }
        }
    }
    
    void Start()
    {
        if (StateManager.instance != null)
        {
            animator.SetFloat("Health", StateManager.instance.currentHealth);
        }
        else
        {
            Debug.LogWarning("StateManager 未找到，使用默认生命值");
            animator.SetFloat("Health", health);
        }
    }
    
    void Update()
    {
        if (behitLayerIndex == -1 || animator == null || hitAnimationNames == null || hitAnimationNames.Count == 0) return;
        
        AnimatorStateInfo behitStateInfo = animator.GetCurrentAnimatorStateInfo(behitLayerIndex);
        bool isHitAnimationPlaying = IsHitAnimation(behitStateInfo);
        string playingHitAnimationName = GetPlayingHitAnimationName(behitStateInfo);
        
        bool isBaseLayerAllowed = IsBaseLayerAnimationAllowed();
        
        if (isHitAnimationPlaying && !isPlayingHitAnimation && !isTransitioning && isBaseLayerAllowed)
        {
            isPlayingHitAnimation = true;
            StartCoroutine(SmoothLayerWeight(0f, 1f, fadeInTime));
            Debug.Log($"{playingHitAnimationName} 开始播放，权重过渡到1");
        }
        else if (!isHitAnimationPlaying && isPlayingHitAnimation && !isTransitioning)
        {
            isPlayingHitAnimation = false;
            StartCoroutine(SmoothLayerWeight(1f, 0f, fadeOutTime));
            Debug.Log($"受击动画播放结束，权重恢复为0");
        }
        else if (isHitAnimationPlaying && !isBaseLayerAllowed && !isTransitioning)
        {
            animator.SetLayerWeight(behitLayerIndex, 0f);
        }
        
        if (Health <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("Dead");
        }
    }
    
    private bool IsBaseLayerAnimationAllowed()
    {
        if (allowedBaseLayerAnimations == null || allowedBaseLayerAnimations.Count == 0)
        {
            return true;
        }
        
        AnimatorStateInfo baseStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        foreach (string name in allowedBaseLayerAnimations)
        {
            if (baseStateInfo.IsName(name))
            {
                return true;
            }
        }
        return false;
    }
    
    private bool IsHitAnimation(AnimatorStateInfo stateInfo)
    {
        foreach (string name in hitAnimationNames)
        {
            if (stateInfo.IsName(name))
            {
                return true;
            }
        }
        return false;
    }
    
    private string GetPlayingHitAnimationName(AnimatorStateInfo stateInfo)
    {
        foreach (string name in hitAnimationNames)
        {
            if (stateInfo.IsName(name))
            {
                return name;
            }
        }
        return "Unknown";
    }
    
    public void TakeDamage(float damageAmount)
    {
        TakeDamage(damageAmount, false);
    }

    public void TakeDamage(float damageAmount, bool isLargeAttack)
    {
        StopAttackEffects();
        
        // 优先减少护盾
        float remainingDamage = ApplyDamageToShield(damageAmount);
        
        // 如果护盾不足以抵挡全部伤害，剩余伤害减少生命值
        if (remainingDamage > 0)
        {
            Health -= remainingDamage;
        }
        
        if (isLargeAttack)
        {
            animator.SetTrigger("BeAttacking_Large");
            Debug.Log("受到大幅攻击！");
        }
        else
        {
            animator.SetTrigger("BeAttacking");
        }
        
        Debug.Log($"当前生命值: {Health}，当前护盾值: {Shield}");
        
        if (Health <= 0)
        {
            Debug.Log("角色死亡！");
        }
    }

    public void TakeDamage_Down(float damageAmount)
    {
        StopAttackEffects();
        
        // 优先减少护盾
        float remainingDamage = ApplyDamageToShield(damageAmount);
        
        // 如果护盾不足以抵挡全部伤害，剩余伤害减少生命值
        if (remainingDamage > 0)
        {
            Health -= remainingDamage;
        }
        
        animator.SetTrigger("Behit_Down");
        Debug.Log("受到击倒攻击！");
        
        Debug.Log($"当前生命值: {Health}，当前护盾值: {Shield}");
        
        if (Health <= 0)
        {
            animator.SetTrigger("Dead");
            Debug.Log("角色死亡！");
        }
    }
    
    private void StopAttackEffects()
    {
        if (comboEffectManager == null) return;
        
        foreach (string key in attackEffectKeys)
        {
            comboEffectManager.StopEffect(key);
        }
    }
    
    private float ApplyDamageToShield(float damageAmount)
    {
        if (Shield <= 0)
        {
            return damageAmount;
        }
        
        if (damageAmount <= Shield)
        {
            Shield -= damageAmount;
            return 0f;
        }
        else
        {
            float remainingDamage = damageAmount - Shield;
            Shield = 0f;
            return remainingDamage;
        }
    }
    
    private IEnumerator SmoothLayerWeight(float startWeight, float targetWeight, float duration)
    {
        isTransitioning = true;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentWeight = Mathf.Lerp(startWeight, targetWeight, t);
            animator.SetLayerWeight(behitLayerIndex, currentWeight);
            yield return null;
        }
        animator.SetLayerWeight(behitLayerIndex, targetWeight);
        isTransitioning = false;
    }
    
    public void OnDeathAnimationEnd()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerRespawn();
            Debug.Log("动画事件触发：调用玩家重生");
        }
        else
        {
            Debug.LogWarning("GameManager.Instance 为空，无法触发重生");
        }
    }
    
    public void ResetDeadState()
    {
        isDead = false;
        Debug.Log("BeAttack: 已重置死亡状态");
    }
}

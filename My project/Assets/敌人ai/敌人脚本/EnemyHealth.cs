using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private Animator animator;
    private EnemyAI enemyAI;
    private CapsuleCollider capsuleCollider; // 1. 声明胶囊碰撞体变量

    // [重要] 请在Inspector面板中拖入你的粒子特效预制体
    public GameObject hitEffectPrefab; 

    [Header("打击感设置")]
    [Tooltip("顿帧时的时间流速，0表示完全暂停，0.1-0.2感觉比较明显")]
    public float hitPauseScale = 0.1f; 
    
    [Tooltip("顿帧持续的时长（秒），例如0.1秒")]
    public float hitPauseDuration = 0.1f;

    public float currentHealth = 100f;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        
        // 2. 获取胶囊碰撞体组件
        // 如果你的敌人使用的是 BoxCollider 或 MeshCollider，请相应修改这里
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // --- 兼容旧代码的函数 ---
    public void TakeHit(int damageAmount, float knockback)
    {
        animator.SetTrigger("Be_hit");
        Debug.Log("受到攻击！伤害值：" + damageAmount);
    }

    // --- 新增功能的函数 ---
    public void TakeHit(int damageAmount, float knockback, Transform attacker, bool isParry = false)
    {
        // 播放动画 - 如果是格挡反击则播放专门的格挡反击受击动画
        if (isParry)
        {
            animator.SetTrigger("Be_Parryhit");
            Debug.Log("受到格挡反击！");
            
            // 如果是Boss，重置所有攻击状态，让Boss进入idle状态
            EnemyAI_Boss bossAI = GetComponent<EnemyAI_Boss>();
            if (bossAI != null)
            {
                bossAI.ResetAttackState();
            }
        }
        else
        {
            animator.SetTrigger("Be_hit");
        }
        
        // 播放特效
        if (hitEffectPrefab != null)
        {
            SpawnHitEffect(attacker);
        }

        // 执行顿帧效果
        StartCoroutine(HitPauseCoroutine());
        
        // 减少生命值
        currentHealth -= damageAmount;
        animator.SetFloat("Enemy_Health", currentHealth);
        Debug.Log("受到攻击！伤害值：" + damageAmount);
        
        if (currentHealth <= 0)
        {
            // --- 敌人死亡逻辑 ---
            animator.SetTrigger("Dead");
            
            // 禁用 AI 逻辑
            if (enemyAI != null)
            {
                enemyAI.enabled = false;
            }

            // 禁用碰撞体，让主角可以直接穿过去
            if (capsuleCollider != null)
            {
                capsuleCollider.enabled = false;
            }
            
            // 如果敌人有 Rigidbody，建议禁用它
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            Debug.Log("敌人死亡！碰撞体已移除。");

            // 延迟两秒销毁对象
            Invoke("DestroyEnemy", 4f);
        }
    }

    private void SpawnHitEffect(Transform attacker)
    {
        Vector3 spawnPos = this.transform.position;
        spawnPos.y += 1.0f; 
        spawnPos += this.transform.forward * 3f;

        GameObject effect = Instantiate(hitEffectPrefab, spawnPos, Quaternion.identity);

        effect.transform.LookAt(attacker);

        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        Destroy(effect, 1.0f);
    }

    private IEnumerator HitPauseCoroutine()
    {
        Time.timeScale = hitPauseScale;
        yield return new WaitForSecondsRealtime(hitPauseDuration);
        Time.timeScale = 1.0f;
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
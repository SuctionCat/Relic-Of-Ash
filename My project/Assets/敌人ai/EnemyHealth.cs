using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private Animator animator;

    // [重要] 请在Inspector面板中拖入你的粒子特效预制体
    public GameObject hitEffectPrefab; 

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // --- 兼容旧代码的函数 (保持参数不变) ---
    // 如果其他地方调用这个函数，它只会播放动画，不会播放特效
    // 这样可以保证你现有的其他脚本不出错
    public void TakeHit(int damageAmount, float knockback)
    {
        animator.SetTrigger("Be_hit");
        Debug.Log("受到攻击！伤害值：" + damageAmount);
        
        // 这里可以放通用的扣血逻辑
    }

    // --- 新增功能的函数 (支持特效和朝向) ---
    // 主角攻击时，请调用这个函数
    public void TakeHit(int damageAmount, float knockback, Transform attacker)
    {
        // 1. 播放动画
        animator.SetTrigger("Be_hit");
        
        // 2. 播放特效 (核心修改点)
        if (hitEffectPrefab != null)
        {
            SpawnHitEffect(attacker);
        }

        Debug.Log("受到攻击！伤害值：" + damageAmount);
        
        // 3. 扣血和击退逻辑...
    }

    /// <summary>
    /// 处理特效生成和朝向的逻辑
    /// </summary>
    private void SpawnHitEffect(Transform attacker)
    {
       // 1. 确定生成位置 (敌人位置)
        Vector3 spawnPos = this.transform.position;
        spawnPos.y += 1.0f; // 稍微抬高一点，防止被地面遮挡
        spawnPos += this.transform.forward * 3f;// 稍微向敌人前面移动一点，防止特效被敌人遮挡

        // 2. 实例化特效
        GameObject effect = Instantiate(hitEffectPrefab, spawnPos, Quaternion.identity);

        // 3. 让特效面向主角
        effect.transform.LookAt(attacker);
        //effect.transform.Rotate(0, 180, 0);

        // 4. 【核心修改】获取粒子组件并强制播放
        // 即使预制体设置了 Play On Awake，手动调用 Play() 也是最稳妥的
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        // 5. 【核心修改】1秒后自动销毁该特效物体
        // 注意：如果你的特效持续时间超过1秒，请适当增加这个时间
        Destroy(effect, 1.0f);
    }
}
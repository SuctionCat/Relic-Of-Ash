using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 10f;  // 定义敌人的伤害值
    public string targetTag = "Player";  // 主角的Tag
    private bool isAttacking = false;  // 控制是否正在攻击
    private bool hasHit = false;  // 确保在攻击期间只造成一次伤害
    private bool isLargeAttack = false;  // 是否是大幅攻击
    private bool isDownAttack = false;  // 是否是击倒攻击
    public Collider attackCollider;  // 武器的碰撞体（胶囊体）

    void Start()
    {
        // 确保在开始时攻击碰撞体是禁用的
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    // 通过动画事件来启动攻击
    public void StartAttack()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;  // 启用碰撞体
        }
        isAttacking = true;
        hasHit = false;  // 重置伤害标志
        isLargeAttack = false;  // 标记为普通攻击
    }

    // 大幅攻击的动画事件调用
    public void StartLargeAttack()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;  // 启用碰撞体
        }
        isAttacking = true;
        hasHit = false;  // 重置伤害标志
        isLargeAttack = true;  // 标记为大幅攻击
        isDownAttack = false;  // 重置击倒攻击标记
    }

    // 击倒攻击的动画事件调用
    public void StartDownAttack()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;  // 启用碰撞体
        }
        isAttacking = true;
        hasHit = false;  // 重置伤害标志
        isDownAttack = true;  // 标记为击倒攻击
        isLargeAttack = false;  // 重置大幅攻击标记
    }

    // 通过动画事件来停止攻击
    public void EndAttack()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;  // 禁用碰撞体
        }
        isAttacking = false;
        isLargeAttack = false;  // 重置大幅攻击标记
        isDownAttack = false;  // 重置击倒攻击标记
    }

    // 检查攻击碰撞
    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && !hasHit && other.CompareTag(targetTag))
        {
            // 根据攻击类型调用不同的函数
            if (isDownAttack)
            {
                EnemyHit_Down(other.gameObject);
            }
            else if (isLargeAttack)
            {
                EnemyHit_Large(other.gameObject);
            }
            else
            {
                EnemyHit(other.gameObject);
            }
            hasHit = true;  // 确保只受到一次伤害
        }
    }

    // 定义EnemyHit函数
    void EnemyHit(GameObject target)
    {
        Debug.Log("Enemy hits player with " + damage + " damage!");

        // 假设主角有一个Health组件来管理生命值
        BeAttack playerHealth = target.GetComponent<BeAttack>();
        if (playerHealth != null)
        {
            // 对主角造成伤害，传递 false 表示普通攻击
            playerHealth.TakeDamage(damage, false);
        }
    }

    // 定义大幅攻击的EnemyHit_Large函数
    void EnemyHit_Large(GameObject target)
    {
        float largeDamage = damage * 1.5f;  // 大幅攻击伤害为普通攻击的1.5倍
        Debug.Log("Enemy hits player with large attack! Damage: " + largeDamage);

        // 假设主角有一个Health组件来管理生命值
        BeAttack playerHealth = target.GetComponent<BeAttack>();
        if (playerHealth != null)
        {
            // 对主角造成大幅伤害，传递 true 表示大幅攻击
            playerHealth.TakeDamage(largeDamage, true);
        }
    }

    // 定义击倒攻击的EnemyHit_Down函数
    void EnemyHit_Down(GameObject target)
    {
        float downDamage = damage * 2f;  // 击倒攻击伤害为普通攻击的2倍
        Debug.Log("Enemy hits player with down attack! Damage: " + downDamage);

        // 假设主角有一个Health组件来管理生命值
        BeAttack playerHealth = target.GetComponent<BeAttack>();
        if (playerHealth != null)
        {
            // 对主角造成击倒伤害，传递 true 表示击倒攻击
            playerHealth.TakeDamage_Down(downDamage);
        }
    }
}
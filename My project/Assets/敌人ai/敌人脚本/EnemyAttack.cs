using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 10f;  // 定义敌人的伤害值
    public string targetTag = "Player";  // 主角的Tag
    private bool isAttacking = false;  // 控制是否正在攻击
    private bool hasHit = false;  // 确保在攻击期间只造成一次伤害
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
    }

    // 通过动画事件来停止攻击
    public void EndAttack()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;  // 禁用碰撞体
        }
        isAttacking = false;
    }

    // 检查攻击碰撞
    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && !hasHit && other.CompareTag(targetTag))
        {
            // 调用EnemyHit函数
            EnemyHit(other.gameObject);
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
            // 对主角造成伤害
            playerHealth.TakeDamage(damage);
        }
    }
}
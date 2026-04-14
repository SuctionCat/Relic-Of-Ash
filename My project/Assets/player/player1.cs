using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player1 : MonoBehaviour
{
    // 基础属性
    public float maxHealth = 100f;
    public float currentHealth;
    public float defense = 10f;
    public float attackDamage = 20f;
    
    // 武器碰撞箱
    public Collider attack1Pos;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 处理武器碰撞
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Enemy1"))
        {
            // 对敌人造成伤害
            Enemy123 enemy = collision.GetComponent<Enemy123>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }
    
    // 玩家受到伤害的方法
    public void TakeDamage(float damage)
    {
        // 计算实际伤害（考虑防御）
        float actualDamage = Mathf.Max(0, damage - defense);
        currentHealth -= actualDamage;
        
        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // 玩家死亡方法
    void Die()
    {
        // 在这里添加死亡逻辑
        Debug.Log("Player died!");
        // 可以添加游戏结束、重生等逻辑
    }
}

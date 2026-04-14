using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy123 : MonoBehaviour
{
    // 敌人基础属性
    public float maxHealth = 50f;
    public float currentHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 处理碰撞检测
    private void OnTriggerEnter(Collider collision)
    {
        // 检测是否与玩家武器碰撞
        if (collision.CompareTag("attckpos"))
        {
            // 获取武器所属的玩家
            player1 player = collision.GetComponentInParent<player1>();
            if (player != null)
            {
                // 受到伤害
                TakeDamage(player.attackDamage);
            }
        }
    }
    
    // 敌人受到伤害的方法
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // 敌人死亡方法
    void Die()
    {
        // 在这里添加死亡逻辑
        Debug.Log("Enemy123 died!");
        // 可以添加敌人死亡动画、消失效果等
        Destroy(gameObject);
    }
}
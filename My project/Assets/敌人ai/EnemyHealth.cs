using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // 接收伤害数值
    public void TakeHit(int damageAmount, float knockback)
    {
        animator.SetTrigger("Be_hit");
        
        // 扣血逻辑
        // currentHealth -= damageAmount;
        
        Debug.Log("受到攻击！伤害值：" + damageAmount);
        
        // 击退逻辑...
    }
}
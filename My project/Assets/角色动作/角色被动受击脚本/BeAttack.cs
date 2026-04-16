using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BeAttack : MonoBehaviour
{

    private Animator animator;// 引用动画组件
    public float Health = 1000f;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public void TakeDamage(float damageAmount)
    {
        Health -= damageAmount;
        animator.SetTrigger("BeAttacking");
        animator.SetFloat("Health", Health);
        Debug.Log($"当前生命值: {Health}");
        if (Health <= 0)
        {
            // 角色死亡
            Debug.Log("角色死亡！");
            //Destroy(gameObject);
        }
    }
}

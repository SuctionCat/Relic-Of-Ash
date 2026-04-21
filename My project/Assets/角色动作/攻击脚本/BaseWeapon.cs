using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这是一个抽象的基类，所有武器都要继承它
public class BaseWeapon : MonoBehaviour
{
    // 1. 定义武器数值
    public int damage = 10; 
    public float knockbackForce = 5f;

    // 2. 引用自身的碰撞体，方便开启/关闭（可选，也可以直接写在 Sword_WeaponHit 里）
    private Collider weaponCollider;

    protected bool canAttack = false;

    // 3. 格挡反击动画列表，在检查器中添加需要判定为格挡反击的动画名称
    [Header("格挡反击设置")]
    public List<string> parryAnimationNames = new List<string>();
    
    // 获取动画组件（假设挂载在父物体或自身）
    private Animator animator;

    protected virtual void Awake()
    {
        // 自动获取挂载在这个物体上的碰撞体
        weaponCollider = GetComponent<Collider>();
        
        // 获取动画组件（优先从父物体查找，因为武器通常是子物体）
        animator = GetComponentInParent<Animator>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public virtual void SetAttackActive(int isActive)
    {
        canAttack = (isActive == 1);
        
        if(weaponCollider != null)
        {
            weaponCollider.enabled = canAttack;
        }
        Debug.Log($"Weapon: {gameObject.name}, Active: {canAttack}, Time: {Time.time}");
    }

    // 检查当前是否在播放格挡反击动画
    private bool IsParryAttack()
    {
        if (animator == null || parryAnimationNames.Count == 0)
            return false;
        
        // 遍历检查器中设置的格挡反击动画列表
        foreach (string animName in parryAnimationNames)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
            {
                return true;
            }
        }
        return false;
    }

    // 公共的触发逻辑
    private void OnTriggerEnter(Collider other)
    {
        // 只有开关打开时才能触发
        if (!canAttack) return;

        // 获取敌人脚本（假设敌人都有一个共同的接口或基类，或者直接用 Tag）
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        
        if (enemy != null)
        {
            // 检查是否是格挡反击
            bool isParry = IsParryAttack();
            
            // 关键点：把“父类”的 damage 传给敌人，并传入是否是格挡反击
            enemy.TakeHit(damage, knockbackForce, this.transform, isParry);
            
            // 防止同一帧多次判定（视具体需求而定，通常建议加个简单的冷却或标记）
            //canAttack = false; 
            //if(weaponCollider != null) weaponCollider.enabled = false;
        }
    }
}

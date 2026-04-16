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

    protected virtual void Awake()
    {
        // 自动获取挂载在这个物体上的碰撞体
        weaponCollider = GetComponent<Collider>();
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

    // 公共的触发逻辑
    private void OnTriggerEnter(Collider other)
    {
        // 只有开关打开时才能触发
        if (!canAttack) return;

        // 获取敌人脚本（假设敌人都有一个共同的接口或基类，或者直接用 Tag）
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        
        if (enemy != null)
        {
            // 关键点：把“父类”的 damage 传给敌人
            enemy.TakeHit(damage, knockbackForce,this.transform);
            
            // 防止同一帧多次判定（视具体需求而定，通常建议加个简单的冷却或标记）
            //canAttack = false; 
            //if(weaponCollider != null) weaponCollider.enabled = false;
        }
    }
}

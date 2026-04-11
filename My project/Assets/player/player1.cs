using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player1 : MonoBehaviour
{
    // 玩家基础属性
    public float maxHealth = 100f;
    public float currentHealth;
    public float attack = 10f;
    public float defense = 5f;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float maxMana = 50f;
    public float currentMana;
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;

    // 攻击相关属性
    public float attackSpeed = 1f; // 攻击速度，越高攻击间隔越短
    private float attackCooldown = 0f; // 攻击冷却时间
    public float attackRange = 2f; // 攻击范围
    public LayerMask enemyLayer; // 敌人图层

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    // Update is called once per frame
    void Update()
    {
        // 攻击冷却时间减少
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // 检测攻击输入
        if (Input.GetButtonDown("Fire1") && attackCooldown <= 0)
        {
            Attack();
        }
    }

    // 攻击方法
    void Attack()
    {
        // 重置攻击冷却时间
        attackCooldown = 1f / attackSpeed;

        // 播放攻击动画（如果有动画组件）
        // GetComponent<Animator>().SetTrigger("Attack");

        // 检测攻击范围内的敌人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        // 对每个敌人造成伤害
        foreach (Collider2D enemy in hitEnemies)
        {
            // 假设敌人有一个Enemy脚本，其中有TakeDamage方法
            // enemy.GetComponent<Enemy>().TakeDamage(attack);
            Debug.Log("攻击命中敌人，造成 " + attack + " 点伤害");
        }
    }

    // 绘制攻击范围（仅在编辑器中可见）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

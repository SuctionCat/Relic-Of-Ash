using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float speed = 3f;
    public float chaseSpeed = 5f;
    public float detectionRadius = 10f;
    public float attackDistance = 2f;
    public Transform player;
    
    // 敌人属性
    public float maxHealth = 100f;
    private float currentHealth;
    public float defense = 10f;
    public float attackSpeed = 1f;
    public float attackDamage = 20f;
    
    private int currentPointIndex = 0;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDead = false;
    private float attackTimer = 0f;

    // 死亡事件
    public System.Action onDeath;

    // Start is called before the first frame update
    void Start()
    {
        if (patrolPoints == null || patrolPoints.Length < 2)
        {
            Debug.LogError("请在Inspector中设置至少两个巡逻点");
        }
        if (player == null)
        {
            Debug.LogError("请在Inspector中设置玩家对象");
        }
        
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;
            
        CheckPlayerDistance();
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
        
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= 1f / attackSpeed)
            {
                AttackPlayer();
                attackTimer = 0f;
            }
        }
    }

    void CheckPlayerDistance()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isChasing = distanceToPlayer < detectionRadius;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length < 2)
            return;

        Transform target = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > attackDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            isAttacking = false;
        }
        else
        {
            isAttacking = true;
        }
    }

    void AttackPlayer()
    {
        // 这里可以添加攻击逻辑
        // 例如播放攻击动画、造成伤害等
        Debug.Log("敌人正在攻击玩家！造成 " + attackDamage + " 点伤害");
        // 这里可以添加对玩家造成伤害的代码
        // 例如：player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;
            
        // 计算实际伤害（考虑防御值）
        float actualDamage = Mathf.Max(1, damage - defense);
        currentHealth -= actualDamage;
        
        Debug.Log("敌人受到 " + actualDamage + " 点伤害，当前生命值：" + currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        isChasing = false;
        isAttacking = false;
        
        Debug.Log("敌人死亡！");
        
        // 触发死亡事件
        if (onDeath != null)
        {
            onDeath.Invoke();
        }
        
        // 这里可以添加死亡动画、粒子效果等
        // 例如：GetComponent<Animator>().SetTrigger("Death");
        
        // 延迟销毁敌人对象
        Destroy(gameObject, 2f);
    }
}

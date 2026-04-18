using UnityEngine;
using UnityEngine.AI; // 1. 必须引入导航命名空间

public class EnemyAI : MonoBehaviour
{
    // --- 组件引用 ---
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform playerTransform;
    protected bool isCurrentlyAttacking = false;// 是否正在攻击，防止攻击时移动

    // --- 配置参数 (可在Inspector面板调整) ---
    public float detectionRange = 10f; // 发现玩家的距离范围
    public float moveSpeed = 3.5f;     // 移动速度
    public float attackRange = 2.0f;   // 攻击距离（这里要和 NavMeshAgent 的“停止距离”保持一致）


    protected virtual void Start()
    {
        // 2. 获取组件
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // 3. 寻找主角 (确保你的主角物体标签Tag设置为 "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("未找到带有 'Player' 标签的主角！");
        }

        // 4. 设置导航代理速度
        agent.speed = moveSpeed;
    }

    protected virtual void Update()
    {
        if (playerTransform == null) return;// 如果主角不存在，直接返回

        float distance = Vector3.Distance(transform.position, playerTransform.position);// 计算敌人到主角的距离

        animator.SetFloat("Distance", distance);// 更新动画参数

        // 1. 发现玩家
        if (distance <= detectionRange)
        {
            animator.SetBool("Find_Player", true);

            // 2. 判断是否在攻击范围内
            if (distance <= attackRange)
            {
                // --- 进入攻击状态 ---
                isCurrentlyAttacking = true;
                
                // 停止移动 (虽然 Agent 会因为“停止距离”自动减速，但显式停止更稳妥)
                agent.isStopped = true; 
                agent.ResetPath(); // 清除路径防止抖动
                
                // 触发攻击动画
                animator.SetBool("canAttack", true);
                
                // (可选) 强制面朝玩家，防止转身
                transform.LookAt(playerTransform);
            }
            else
            {
                // --- 进入追击状态 ---
                
                // 如果正在攻击中，即使距离远了，也不要执行追击逻辑（不要开启移动）
                if (isCurrentlyAttacking)
                {
                    // 此时什么都不做，保持原地不动，等待动画结束
                    return; 
                }

                // 确保不在攻击状态
                animator.SetBool("canAttack", false);
                
                // 恢复移动
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
            }
        }
        else
        {
            // 3. 未发现玩家 (待机)
            animator.SetBool("Find_Player", false);
            animator.SetBool("canAttack", false);
            agent.isStopped = true;
            // 如果跑太远，重置攻击状态（防止卡死）
            isCurrentlyAttacking = false;
        }
    }
    public virtual void FinishAttackAnimation()
    {
        isCurrentlyAttacking = false;
        // 可选：如果攻击结束时玩家还在范围内且没跑远，可以立刻恢复追击
        // 这里为了稳妥，只是重置标志位，等待下一帧 Update 重新判断
    }
}
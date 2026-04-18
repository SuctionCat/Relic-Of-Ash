using System.Collections;
using UnityEngine;

public class EnemyAI_Boss : EnemyAI
{
    // 攻击类型索引，0表示canAttack1，1表示canAttack2，用于循环切换
    private int attackIndex = 0;
    
    // 第三个攻击的冷却时间（秒）
    public float attack3Cooldown = 20f;
    
    // 第三个攻击的冷却计时器
    private float attack3Timer = 0f;
    
    // 第三个攻击是否就绪
    private bool isAttack3Ready = true;
    
    // Attack1和Attack2的冷却时间范围（秒）
    public float minAttackCooldown = 1f;
    public float maxAttackCooldown = 2f;
    
    // 攻击冷却计时器
    private float attackCooldownTimer = 0f;
    
    // Attack1和Attack2是否就绪
    private bool isAttackReady = true;
    
    // 当前攻击冷却时间
    private float currentAttackCooldown = 0f;
    
    // 追击计时器
    private float chaseTimer = 0f;
    
    // 追击时间阈值（秒）- 追击这么长时间没攻击到玩家就触发特殊攻击
    public float chaseTimeThreshold = 10f;
    
    // 是否正在执行特殊攻击（翻滚+刺击）
    private bool isPerformingSpecialAttack = false;
    
    // 是否正在转向玩家（用于特殊攻击）
    private bool isTurningToPlayer = false;
    
    // 武器胶囊体引用
    public CapsuleCollider weaponCollider;
    
    // 武器胶囊体原始长度
    private float originalWeaponLength;
    
    // 武器胶囊体原始半径
    private float originalWeaponRadius;
    
    // 刺击时武器胶囊体的长度
    public float extendedWeaponLength = 10f;
    
    // 刺击时武器胶囊体的半径
    public float extendedWeaponRadius = 1.5f;
    
    // 追击行为类型
    private enum ChaseBehavior
    {
        DirectChase,    // 直接追击
        CircleAround,   // 绕圈移动
        Standoff        // 对峙（原地不动但面朝玩家）
    }
    
    // 当前追击行为
    private ChaseBehavior currentChaseBehavior;
    
    // 当前行为持续时间
    private float currentBehaviorDuration;
    
    // 行为计时器
    private float behaviorTimer;
    
    // 绕圈移动的半径（相对于攻击范围）
    public float circleRadiusMultiplier = 1.2f;
    
    // 绕圈移动的角度
    private float circleAngle;
    
    // 行为切换的最小和最大间隔时间（秒）
    public float minBehaviorDuration = 2f;
    public float maxBehaviorDuration = 5f;

    // 改写 Start 方法
    protected override void Start()
    {
        base.Start();
        
        // 保存武器胶囊体原始长度和半径
        if (weaponCollider != null)
        {
            originalWeaponLength = weaponCollider.height;
            originalWeaponRadius = weaponCollider.radius;
        }
    }

    // 改写 Update 方法
    protected override void Update()
    {
        if (playerTransform == null) return;// 如果主角不存在，直接返回

        float distance = Vector3.Distance(transform.position, playerTransform.position);// 计算敌人到主角的距离

        animator.SetFloat("Distance", distance);// 更新动画参数

        // 更新第三个攻击的冷却计时器
        if (!isAttack3Ready)
        {
            attack3Timer += Time.deltaTime;
            if (attack3Timer >= attack3Cooldown)
            {
                isAttack3Ready = true;
                attack3Timer = 0f;
            }
        }
        
        // 更新Attack1和Attack2的冷却计时器
        if (!isAttackReady)
        {
            attackCooldownTimer += Time.deltaTime;
            if (attackCooldownTimer >= currentAttackCooldown)
            {
                isAttackReady = true;
                attackCooldownTimer = 0f;
            }
        }

        // 1. 发现玩家
        if (distance <= detectionRange)
        {
            animator.SetBool("Find_Player", true);
            
            // 2. 判断是否在攻击范围内
            if (distance <= attackRange)
            {
                // 如果正在攻击中，跳过攻击触发逻辑，避免重复触发
                if (isCurrentlyAttacking)
                {
                    return;
                }
                
                // 计算当前面向玩家的角度
                Vector3 targetDirection = playerTransform.position - transform.position;
                targetDirection.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                float angleDiff = Quaternion.Angle(transform.rotation, targetRotation);
                
                // 如果角度差超过10度，先转向玩家
                if (angleDiff > 10f)
                {
                    // 平滑转向玩家
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
                    // 停止移动，准备攻击
                    agent.isStopped = true;
                    agent.ResetPath();
                    animator.SetBool("Stand", false);
                    return; // 等待转向完成后再攻击
                }
                
                // 退出对峙状态
                animator.SetBool("Stand", false);
                
                // 触发攻击动画 - 优先使用第三个攻击（如果冷却完毕）
                if (isAttack3Ready)
                {
                    // --- 进入攻击状态 ---
                    isCurrentlyAttacking = true;
                    
                    // 停止移动
                    agent.isStopped = true; 
                    agent.ResetPath();
                    
                    animator.SetBool("canAttack3", true);
                    isAttack3Ready = false;
                    attack3Timer = 0f;
                    // 攻击3也触发Attack1和Attack2的冷却
                    StartAttackCooldown();
                }
                else if (isAttackReady && attackIndex == 0)
                {
                    // --- 进入攻击状态 ---
                    isCurrentlyAttacking = true;
                    
                    // 停止移动
                    agent.isStopped = true; 
                    agent.ResetPath();
                    
                    animator.SetBool("canAttack1", true);
                    attackIndex = 1;
                    StartAttackCooldown();
                }
                else if (isAttackReady && attackIndex == 1)
                {
                    // --- 进入攻击状态 ---
                    isCurrentlyAttacking = true;
                    
                    // 停止移动
                    agent.isStopped = true; 
                    agent.ResetPath();
                    
                    animator.SetBool("canAttack2", true);
                    attackIndex = 0;
                    StartAttackCooldown();
                }
                else
                {
                    // 所有攻击都在冷却中，继续追击玩家
                    agent.isStopped = false;
                    agent.SetDestination(playerTransform.position);
                    
                    // 确保退出站立状态
                    animator.SetBool("Stand", false);
                }
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

                    // 如果正在执行特殊攻击
                    if (isPerformingSpecialAttack)
                    {
                        // 如果正在转向玩家
                        if (isTurningToPlayer)
                        {
                            // 计算当前面向玩家的角度
                            Vector3 targetDirection = playerTransform.position - transform.position;
                            targetDirection.y = 0;
                            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                            float angleDiff = Quaternion.Angle(transform.rotation, targetRotation);
                            
                            // 如果角度差超过5度，继续转向
                            if (angleDiff > 5f)
                            {
                                // 平滑转向玩家
                                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                                return;
                            }
                            else
                            {
                                // 转向完成，开始长刺击
                                isTurningToPlayer = false;
                                
                                // 触发长刺击动画
                                animator.SetBool("LongStab", true);
                                
                                // 延长武器胶囊体
                                if (weaponCollider != null)
                                {
                                    weaponCollider.height = extendedWeaponLength;
                                    weaponCollider.radius = extendedWeaponRadius;
                                }
                            }
                        }
                        return;
                    }

                    // 确保不在攻击状态
                    animator.SetBool("canAttack1", false);
                    animator.SetBool("canAttack2", false);
                    animator.SetBool("canAttack3", false);
                    
                    // 如果攻击正在冷却中，继续追击玩家
                    if (!isAttackReady || !isAttack3Ready)
                    {
                        // 继续追击玩家
                        agent.isStopped = false;
                        agent.SetDestination(playerTransform.position);
                        animator.SetBool("Stand", false);
                        
                        // 冷却期间仍然更新追击计时器
                        chaseTimer += Time.deltaTime;
                        if (chaseTimer >= chaseTimeThreshold)
                        {
                            TriggerSpecialAttack();
                        }
                        return;
                    }
                    
                    // 更新追击计时器
                    chaseTimer += Time.deltaTime;
                    
                    // 检查是否追击时间过长，需要触发特殊攻击
                    if (chaseTimer >= chaseTimeThreshold)
                    {
                        TriggerSpecialAttack();
                        return;
                    }
                    
                    // 更新行为计时器
                    behaviorTimer += Time.deltaTime;
                    
                    // 检查是否需要切换行为
                    if (behaviorTimer >= currentBehaviorDuration)
                    {
                        SwitchChaseBehavior();
                    }
                    
                    // 根据当前行为执行不同的移动逻辑
                    ExecuteChaseBehavior();
                }
        }
        else
        {
            // 3. 未发现玩家 (待机)
            animator.SetBool("Find_Player", false);
            animator.SetBool("canAttack1", false);
            animator.SetBool("canAttack2", false);
            animator.SetBool("canAttack3", false);
            animator.SetBool("Stand", false); // 退出对峙状态
            agent.isStopped = true;
            // 如果跑太远，重置攻击状态（防止卡死）
            isCurrentlyAttacking = false;
        }
    }

    // 当攻击动作完成时调用这个方法
    public override void FinishAttackAnimation()
    {
        isCurrentlyAttacking = false;
        animator.SetBool("canAttack1", false);
        animator.SetBool("canAttack2", false);
        animator.SetBool("canAttack3", false);
        
        // 攻击完成后重置追击计时器
        chaseTimer = 0f;
    }
    
    // 触发特殊攻击（翻滚+长刺击）
    private void TriggerSpecialAttack()
    {
        // 停止移动
        agent.isStopped = true;
        agent.ResetPath();
        
        // 设置正在执行特殊攻击
        isPerformingSpecialAttack = true;
        
        // 重置追击计时器
        chaseTimer = 0f;
        
        // 触发翻滚动画
        animator.SetBool("Roll", true);
    }
    
    // 翻滚动画完成时调用（由动画事件调用）
    public void FinishRollAnimation()
    {
        // 关闭翻滚动画
        animator.SetBool("Roll", false);
        
        // 设置正在转向玩家
        isTurningToPlayer = true;
    }
    
    // 长刺击动画完成时调用（由动画事件调用）
    public void FinishLongStabAnimation()
    {
        // 关闭长刺击动画
        animator.SetBool("LongStab", false);
        
        // 恢复武器胶囊体原始长度和半径
        if (weaponCollider != null)
        {
            weaponCollider.height = originalWeaponLength;
            weaponCollider.radius = originalWeaponRadius;
        }
        
        // 完成特殊攻击
        isPerformingSpecialAttack = false;
    }
    
    // 开始Attack1和Attack2的冷却
    private void StartAttackCooldown()
    {
        isAttackReady = false;
        currentAttackCooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        attackCooldownTimer = 0f;
    }
    
    // 切换追击行为
    private void SwitchChaseBehavior()
    {
        // 如果之前是对峙状态，设置Stand为false
        if (currentChaseBehavior == ChaseBehavior.Standoff)
        {
            animator.SetBool("Stand", false);
        }
        
        // 随机选择下一个行为
        int random = Random.Range(0, 3);
        currentChaseBehavior = (ChaseBehavior)random;
        
        // 设置新的行为持续时间
        currentBehaviorDuration = Random.Range(minBehaviorDuration, maxBehaviorDuration);
        behaviorTimer = 0f;
        
        // 如果是绕圈行为，初始化角度（基于当前位置）
        if (currentChaseBehavior == ChaseBehavior.CircleAround)
        {
            Vector3 direction = transform.position - playerTransform.position;
            direction.y = 0;
            circleAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }
    }
    
    // 执行追击行为
    private void ExecuteChaseBehavior()
    {
        switch (currentChaseBehavior)
        {
            case ChaseBehavior.DirectChase:
                // 直接追击玩家
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
                break;
                
            case ChaseBehavior.CircleAround:
                // 绕圈移动
                agent.isStopped = false;
                
                // 更新绕圈角度
                circleAngle += Time.deltaTime * 20f; // 绕圈速度
                
                // 计算绕圈目标点
                float circleRadius = attackRange * circleRadiusMultiplier;
                Vector3 circleCenter = playerTransform.position;
                Vector3 targetPosition = circleCenter + new Vector3(
                    Mathf.Sin(circleAngle * Mathf.Deg2Rad) * circleRadius,
                    0,
                    Mathf.Cos(circleAngle * Mathf.Deg2Rad) * circleRadius
                );
                
                // 设置目标点
                agent.SetDestination(targetPosition);
                break;
                
            case ChaseBehavior.Standoff:
                // 对峙 - 原地不动但面朝玩家
                agent.isStopped = true;
                agent.ResetPath();
                
                // 设置对峙动画条件为真
                animator.SetBool("Stand", true);
                
                // 平滑面朝玩家
                Vector3 targetDirection = playerTransform.position - transform.position;
                targetDirection.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                break;
        }
    }
}
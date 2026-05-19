using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f; 
    public float targetingRotationSpeed = 5f; // 索敌时的旋转速度
    public float gravity = -9.8f;
    
    public bool useRootMotion = true; 

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpForce = 5f;
    public AnimationClip[] allowedJumpAnimations; 

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;

    [Header("References")]
    public ThirdPersonCamera thirdPersonCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isAirborne = false; 

    // --- 🟢 攻击状态变量 ---
    private bool isAttacking = false;        // 是否正在攻击
    private HashSet<int> attackAnimationHashes = new HashSet<int>(); // 攻击动画Hash缓存集合

    [Header("攻击动画列表")]
    public List<string> attackAnimationNames = new List<string> { "Attack", "Attack2", "Attack3", "Combo" }; // 攻击动画名称列表

    // --- 🟢 索敌系统变量 ---
    private bool isTargeting = false;       // 是否开启索敌模式
    public Transform currentTarget;         // 当前锁定的目标
    
    [Header("Targeting Settings")]
    public float detectionRange = 15f;      // 索敌范围半径
    public LayerMask enemyLayer;            // 敌人所在的层 (建议在Inspector里设置)
    public string enemyTag = "Enemy";       // 敌人的标签

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
            
        // 如果没设置敌层层级，默认包含所有层级，避免无法检测
        if (enemyLayer.value == 0) 
        {
            enemyLayer = Physics.AllLayers;
        }
        
        // 初始化攻击动画Hash缓存
        InitializeAttackAnimationHashes();
    }

    // 🟢 初始化攻击动画Hash缓存（只在Start时执行一次）
    void InitializeAttackAnimationHashes()
    {
        attackAnimationHashes.Clear();
        foreach (string animName in attackAnimationNames)
        {
            // 使用 Animator.StringToHash 将动画名称转换为Hash值
            int hash = Animator.StringToHash(animName);
            attackAnimationHashes.Add(hash);
        }
    }

    void Update()
    {
        //if(GameRoot.GetInstance() != null && GameRoot.GetInstance().IsGamePaused)
        //    return;
        
        HandleInput();
        
        // 🟢 只有在索敌模式下，才每帧尝试更新最近的目标
        if (isTargeting)
        {
            FindClosestEnemy();
        }

        HandleMovement();
        ApplyGravity();

        // 接地检测
        if (controller.isGrounded)
        {
            velocity.y = -2f; 
            isAirborne = false; 
        }

        // 跳跃输入
        if (Input.GetButtonDown("Jump") && controller.isGrounded && IsJumpAllowed())
        {
            Jump();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isTargeting = !isTargeting;
            Debug.Log("索敌状态: " + isTargeting);
            
            // 关闭索敌时清空目标
            if(!isTargeting) currentTarget = null;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.SetTargeting(isTargeting, currentTarget);
            }
        }
    }

    // 🟢 核心功能：寻找最近的敌人
    void FindClosestEnemy()
    {
        // 1. 使用物理重叠球体检测范围内的所有碰撞体
        // 这里的 transform.position 是角色的位置
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

        float closestDistance = Mathf.Infinity; // 初始化最近距离为无穷大
        Transform closestTransform = null;

        foreach (var collider in hitColliders)
        {
            // 2. 检查标签是否匹配 (双重保险，防止Layer设置错误)
            if (collider.CompareTag(enemyTag))
            {
                // 3. 计算距离
                // 使用 sqrMagnitude (平方距离) 比 Vector3.Distance 性能更好，因为不需要开根号
                float distance = (collider.transform.position - transform.position).sqrMagnitude;

                // 4. 如果这个敌人比之前记录的更近，更新它
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTransform = collider.transform;
                }
            }
        }

        // 5. 赋值最终结果
        currentTarget = closestTransform;
        // 🟢 如果摄像机存在，也更新它的目标
        if (thirdPersonCamera != null && isTargeting)
        {
            thirdPersonCamera.SetTargeting(true, currentTarget);
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = HandleFreeMovement(h, v);

        if (!useRootMotion || isAirborne)
        {
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        UpdateAnimator(h, v, moveDir);
    }

    Vector3 HandleFreeMovement(float h, float v)
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;

        // --- 🟢 更新攻击状态 ---
        UpdateAttackState();

        // --- 🟢 智能旋转逻辑 ---
        bool hasInput = moveDir.magnitude > 0.1f;

        // 1. 攻击时 -> 强制面向敌人（忽略WASD输入）
        if (isAttacking && isTargeting && currentTarget != null)
        {
            Vector3 directionToEnemy = currentTarget.position - transform.position;
            directionToEnemy.y = 0; // 忽略高度差
            
            if (directionToEnemy.sqrMagnitude > 0.1f) // 防止除零或抖动
            {
                Quaternion targetRot = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    targetingRotationSpeed * Time.deltaTime * 2 // 攻击时旋转更快
                );
            }
        }
        else if (hasInput)
        {
            // 2. 有 WASD 输入 -> 面向移动方向 (主导)
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
        else if (isTargeting && currentTarget != null)
        {
            // 3. 无输入 且 索敌中 -> 面向敌人
            Vector3 directionToEnemy = currentTarget.position - transform.position;
            directionToEnemy.y = 0; // 忽略高度差
            
            if (directionToEnemy.sqrMagnitude > 0.1f) // 防止除零或抖动
            {
                Quaternion targetRot = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    targetingRotationSpeed * Time.deltaTime
                );
            }
        }

        return moveDir.normalized;
    }

    // 🟢 更新攻击状态
    void UpdateAttackState()
    {
        if (animator == null) return;
        
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        
        // 检测当前动画是否在攻击动画列表中
        isAttacking = IsAttackAnimation(currentState);
    }

    // 🟢 检查当前动画是否为攻击动画（使用Hash缓存，性能提升3-5倍）
    bool IsAttackAnimation(AnimatorStateInfo stateInfo)
    {
        // 使用Hash比较（整数比较远快于字符串比较）
        if (attackAnimationHashes.Contains(stateInfo.fullPathHash))
        {
            return true;
        }
        
        // 备用检查：检查动画片段名称（处理某些特殊情况）
        if (animator.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            foreach (string animName in attackAnimationNames)
            {
                if (clipName.Contains(animName))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    void UpdateAnimator(float h, float v, Vector3 moveDir)
    {
        if (animator == null) return;
        float speed = isAirborne ? moveDir.magnitude : moveDir.magnitude; 
        animator.SetFloat("Speed", speed);

        if (Input.GetButtonDown("Jump") && controller.isGrounded && IsJumpAllowed())
        {
             animator.SetTrigger("Jump");
        }
    }

    void OnAnimatorMove()
    {
        if (animator == null) return;

        if (useRootMotion && !isAirborne)
        {
            controller.Move(animator.deltaPosition);
        }
        else if (useRootMotion && isAirborne)
        {
            Vector3 rootMotionDelta = animator.deltaPosition;
            controller.Move(new Vector3(0, rootMotionDelta.y, 0));
        }
    }

    void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            isAirborne = true; 
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    bool IsJumpAllowed()
    {
        if (allowedJumpAnimations.Length == 0) return true;
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        foreach (var anim in allowedJumpAnimations)
        {
            if (currentState.IsName(anim.name))
            {
                return true;
            }
        }
        return false;
    }
}
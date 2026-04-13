using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.8f;
    
    // 全局开关：是否使用根运动（针对非跳跃状态）
    public bool useRootMotion = true; 

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpForce = 5f;
    public AnimationClip[] allowedJumpAnimations; 

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    
    // 🟢 新增：标记当前是否处于“空中自由控制”状态
    private bool isAirborne = false; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleMovement();
        ApplyGravity();

        // 接地检测
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
            // 🟢 落地时重置状态
            isAirborne = false; 
        }

        // 跳跃输入检测
        if (Input.GetButtonDown("Jump") && controller.isGrounded && IsJumpAllowed())
        {
            Jump();
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = HandleFreeMovement(h, v);

        // ⚠️ 修改点：只有当“不在空中” 且 “开启根运动”时，才不手动移动（交给OnAnimatorMove处理）
        // 如果 isAirborne 为 true，我们需要在这里手动移动，因为 OnAnimatorMove 会忽略 Y 轴以外的根运动
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

        // 旋转逻辑
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        return moveDir.normalized;
    }

    void UpdateAnimator(float h, float v, Vector3 moveDir)
    {
        if (animator == null) return;

        // 根据状态决定速度参数，跳跃时通常保持速度或减速，这里保持原逻辑
        float speed = isAirborne ? moveDir.magnitude : moveDir.magnitude; 
        animator.SetFloat("Speed", speed);

        if (Input.GetButtonDown("Jump") && controller.isGrounded && IsJumpAllowed())
        {
             animator.SetTrigger("Jump");
        }
    }

    // 🟢 核心修改：OnAnimatorMove 是处理根运动的关键
    void OnAnimatorMove()
    {
        if (animator == null) return;

        // 情况1：如果开启了根运动 且 当前 NOT 在空中 -> 完全应用根运动
        if (useRootMotion && !isAirborne)
        {
            controller.Move(animator.deltaPosition);
        }
        // 情况2：如果开启了根运动 但 当前在空中 -> 只应用垂直(Y)根运动，忽略水平(XZ)根运动
        else if (useRootMotion && isAirborne)
        {
            Vector3 rootMotionDelta = animator.deltaPosition;
            // 只保留 Y 轴的根运动（比如跳跃动画本身的上冲力），丢弃 XZ 位移
            controller.Move(new Vector3(0, rootMotionDelta.y, 0));
        }
        // 情况3：如果没开启根运动 -> 不在此处处理，完全由 Update 中的 Move 控制
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
            // 🟢 标记进入空中状态
            isAirborne = true; 
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    bool IsJumpAllowed()
    {
        if (allowedJumpAnimations.Length == 0) return true; // 如果没有指定，默认允许

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
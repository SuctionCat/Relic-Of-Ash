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
    public bool useRootMotion = true;
    public float jumpHeight = 2f;  // 跳跃高度
    public float jumpForce = 5f;   // 跳跃力

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;
    public ThirdPersonCamera cameraController;

    private CharacterController controller;
    private Vector3 velocity;
    private bool Jumping = false;

    // =========================
    // 🟢 允许跳跃的动画
    // =========================
    [Header("Jump Settings")]
    public AnimationClip[] allowedJumpAnimations; // 允许跳跃的动画Clip

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

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // 使角色立刻落地
            Jumping = false;   // 重置Jumping状态
        }

        // 检查当前的动画状态
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        // 如果当前动画是允许的跳跃动画，才允许跳跃
        if (Input.GetButtonDown("Jump") && controller.isGrounded && IsJumpAllowed(currentState))
        {
            Jump();
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = HandleFreeMovement(h, v);

        if (!useRootMotion)
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

        animator.SetFloat("Speed", moveDir.magnitude);

        if (Jumping)
        {
            animator.SetTrigger("Jump");
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

    void OnAnimatorMove()
    {
        if (useRootMotion && animator != null)
        {
            Vector3 rootMotionDelta = animator.deltaPosition;
            rootMotionDelta.y = 0;
            controller.Move(rootMotionDelta);
        }
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            Jumping = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // 检查当前动画是否是允许的跳跃动画
    bool IsJumpAllowed(AnimatorStateInfo currentState)
    {
        foreach (var anim in allowedJumpAnimations)
        {
            if (currentState.IsName(anim.name))
            {
                return true; // 如果当前动画是允许的跳跃动画，则返回 true
            }
        }
        return false; // 否则返回 false
    }
}
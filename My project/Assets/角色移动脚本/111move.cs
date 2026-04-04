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

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;
    public ThirdPersonCamera cameraController;

    private CharacterController controller;
    private Vector3 velocity;

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
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = HandleFreeMovement(h, v);

        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        UpdateAnimator(h, v, moveDir);
    }

    // =========================
    // 🟢 自由状态
    // =========================
    Vector3 HandleFreeMovement(float h, float v)
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;

        // 有输入才转向
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

    // =========================
    // 🎬 Animator同步
    // =========================
    void UpdateAnimator(float h, float v, Vector3 moveDir)
    {
        if (animator == null) return;

        animator.SetBool("IsLocked", false); // 不再有锁敌状态

        // 自由：转换为本地空间
        Vector3 localMove = transform.InverseTransformDirection(moveDir);

        animator.SetFloat("MoveX", localMove.x);
        animator.SetFloat("MoveZ", localMove.z);

        animator.SetFloat("Speed", moveDir.magnitude);
    }

    // =========================
    // 🌍 重力
    // =========================
    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
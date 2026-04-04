using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5.0f;
    public float rotationSmoothTime = 0.1f; // 转身平滑度 (越小越快)
    
    [Header("重力设置")]
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalVelocity;
    
    // 用于平滑旋转
    private float currentRotationVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // 锁定光标（可选）
        // Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        // 1. 获取 WASD 输入
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        // 2. 应用重力
        if (controller.isGrounded)
        {
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 3. 如果有输入，进行移动和旋转
        if (x != 0 || z != 0)
        {
            // 关键逻辑：获取摄像机的前方和右方向量
            // 这样 W 永远是朝着摄像机面对的方向走
            Camera mainCamera = Camera.main;
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            // 忽略 Y 轴，防止角色钻地或飞天
            forward.y = 0f;
            forward.Normalize();
            right.y = 0f;
            right.Normalize();

            // 计算最终的移动方向
            Vector3 moveDirection = (forward * z + right * x).normalized;

            // 4. 移动角色
            velocity = moveDirection * moveSpeed;
            controller.Move(velocity * Time.deltaTime);

            // 5. 平滑旋转角色面向移动方向
            // 计算目标角度
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            // 使用 Slerp 进行平滑插值
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime * 10 * Time.deltaTime);
        }
        else
        {
            // 如果没有输入，只应用重力
            velocity = Vector3.zero;
        }

        // 应用垂直重力
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}
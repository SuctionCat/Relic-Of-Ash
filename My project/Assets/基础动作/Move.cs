using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveDirection;
    
    [Header("移动设置")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 7f;
    public float gravity = 10f;
    
    private float currentSpeed;
    private bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        // 检查并添加CharacterController组件
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            // 设置默认值
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 1, 0);
        }
        currentSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // 获取输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // 奔跑控制
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = true;
            currentSpeed = runSpeed;
        }
        else
        {
            isRunning = false;
            currentSpeed = walkSpeed;
        }
        
        // 计算移动方向
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        
        // 应用重力
        moveDirection.y -= gravity * Time.deltaTime;
        
        // 合并移动和重力
        Vector3 totalMovement = move * currentSpeed * Time.deltaTime + moveDirection * Time.deltaTime;
        controller.Move(totalMovement);
        
        // 跳跃控制 - 确保在地面上时才能跳跃
        if (controller.isGrounded)
        {
            // 重置垂直速度
            moveDirection.y = -0.5f; // 小的负值确保与地面接触
            
            // 检查跳跃输入
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }
    }
}

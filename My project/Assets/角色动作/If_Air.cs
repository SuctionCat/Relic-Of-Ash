using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isGrounded;
    public float groundCheckDistance = 1.0f;  // 检查距离（从脚部到地面的距离）
    public LayerMask groundLayer;              // 地面层，用于检测与地面的碰撞

    void Update()
    {
        CheckIfGrounded();
        
        if (isGrounded)
        {
            // 玩家在地面上
            Debug.Log("Player is grounded");
        }
        else
        {
            // 玩家在空中
            Debug.Log("Player is in the air");
        }
    }

    // 检测是否在地面上
    void CheckIfGrounded()
    {
        // 从角色的位置向下射出射线，检查是否接触到地面
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;  // 射线从角色的当前位置发射
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            // 如果射线撞到了地面
            isGrounded = true;
        }
        else
        {
            // 如果射线没有撞到地面
            isGrounded = false;
        }
    }
}
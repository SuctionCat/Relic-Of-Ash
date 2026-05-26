using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isGrounded;
    public float groundCheckDistance = 1.0f;  // 检查距离（从脚部到地面的距离）
    public LayerMask groundLayer;              // 地面层，用于检测与地面的碰撞
    private Animator animator;

    public float checkWidth = 0.5f;  // 检查角色脚宽的半径（可以根据角色的实际大小调整）
    public float footOffset = 0.5f;  // 从角色中心到脚部的垂直偏移距离（向下为正）

    public Vector3 boxSize = new Vector3(0.4f, 0f, 0.4f);  // BoxCast的大小，表示要检测的区域
    public float sideRayHeight = 0.2f;  // 侧边射线向上偏移的高度，适配胶囊体形状

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckIfGrounded();

        if (isGrounded)
        {
            // 玩家在地面上
            animator.SetBool("If_Air", false);
        }
        else
        {
            // 玩家在空中
            animator.SetBool("If_Air", true);
        }
    }

    // 检测是否在地面上
    void CheckIfGrounded()
    {
        // 使用多条射线检测地面，从角色脚部位置向下发射
        Vector3 footPosition = transform.position + Vector3.down * footOffset;
        
        // 定义射线发射点（3x3正方形排列，共9个检测点）
        float halfWidth = checkWidth * 0.4f;
        float halfDepth = checkWidth * 0.4f; // 使用脚宽作为深度
        Vector3 sideOffset = Vector3.up * sideRayHeight; // 侧边射线向上偏移，适配胶囊体形状
        Vector3[] rayOrigins = new Vector3[] {
            footPosition + Vector3.left * halfWidth + Vector3.forward * halfDepth + sideOffset,  // 左前
            footPosition + Vector3.left * halfWidth + sideOffset,                                  // 左中
            footPosition + Vector3.left * halfWidth + Vector3.back * halfDepth + sideOffset,       // 左后
            footPosition + Vector3.forward * halfDepth + sideOffset,                               // 前中
            footPosition,                                                             // 中心（不偏移）
            footPosition + Vector3.back * halfDepth + sideOffset,                                  // 后中
            footPosition + Vector3.right * halfWidth + Vector3.forward * halfDepth + sideOffset,   // 右前
            footPosition + Vector3.right * halfWidth + sideOffset,                                 // 右中
            footPosition + Vector3.right * halfWidth + Vector3.back * halfDepth + sideOffset       // 右后
        };
        
        foreach (Vector3 origin in rayOrigins)
        {
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
            {
                // 检测到地面，检查斜坡角度
                if (Vector3.Angle(hit.normal, Vector3.up) < 45f)
                {
                    isGrounded = true;
                    return;
                }
            }
        }
        
        isGrounded = false;
    }

    // 可视化射线检测范围
    void OnDrawGizmos()
    {
        // 显示脚部位置
        Vector3 footPosition = transform.position + Vector3.down * footOffset;
        
        // 定义射线发射点（3x3正方形排列）
        float halfWidth = checkWidth * 0.4f;
        float halfDepth = checkWidth * 0.4f;
        Vector3 sideOffset = Vector3.up * sideRayHeight;
        Vector3[] rayOrigins = new Vector3[] {
            footPosition + Vector3.left * halfWidth + Vector3.forward * halfDepth + sideOffset,
            footPosition + Vector3.left * halfWidth + sideOffset,
            footPosition + Vector3.left * halfWidth + Vector3.back * halfDepth + sideOffset,
            footPosition + Vector3.forward * halfDepth + sideOffset,
            footPosition,
            footPosition + Vector3.back * halfDepth + sideOffset,
            footPosition + Vector3.right * halfWidth + Vector3.forward * halfDepth + sideOffset,
            footPosition + Vector3.right * halfWidth + sideOffset,
            footPosition + Vector3.right * halfWidth + Vector3.back * halfDepth + sideOffset
        };
        
        // 绘制射线起点（绿色球）
        Gizmos.color = Color.green;
        foreach (Vector3 origin in rayOrigins)
        {
            Gizmos.DrawWireSphere(origin, 0.05f);
        }
        
        // 绘制射线（蓝色线）
        Gizmos.color = Color.blue;
        foreach (Vector3 origin in rayOrigins)
        {
            Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);
        }
    }
}
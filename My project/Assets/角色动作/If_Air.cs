using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isGrounded;
    public float groundCheckDistance = 1.0f;  // 检查距离（从脚部到地面的距离）
    public LayerMask groundLayer;              // 地面层，用于检测与地面的碰撞
    private Animator animator;

    public float checkWidth = 0.5f;  // 检查角色脚宽的半径（可以根据角色的实际大小调整）

    public Vector3 boxSize = new Vector3(0.4f, 0f, 0.4f);  // BoxCast的大小，表示要检测的区域

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
        // 使用BoxCast检测玩家下方的区域
        Vector3 boxOrigin = transform.position; // 从角色位置开始
        if (Physics.BoxCast(boxOrigin, boxSize / 2, Vector3.down, out RaycastHit hit, Quaternion.identity, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    // 可视化BoxCast范围
    void OnDrawGizmos()
    {
        // 如果你需要看到BoxCast的范围，可以在场景视图中显示它
        Gizmos.color = Color.red; // 设置颜色为红色
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckDistance / 2, boxSize);  // 绘制一个框
    }
}
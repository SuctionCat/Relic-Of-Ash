using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("组件引用")]
    private CharacterController _characterController;
    private Animator _animator;

    [Header("调试设置")]
    [Tooltip("是否在Scene视图中绘制速度向量")]
    public bool drawDebugGizmos = true;
    
    [Header("动画参数")]
    // 这里对应 Animator Controller 里的参数名
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    // 用于在Inspector面板显示的只读变量（方便查看）
    [SerializeField] private float _currentSpeed = 0f;

    private void Awake()
    {
        // 获取组件
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        if (_characterController == null || _animator == null)
        {
            Debug.LogError("玩家身上必须挂载 CharacterController 和 Animator 组件！");
            enabled = false;
        }
    }

    private void Update()
    {
        HandleAnimationSpeed();
    }

    /// <summary>
    /// 处理动画速度逻辑
    /// </summary>
    private void HandleAnimationSpeed()
    {
        // 1. 获取水平移动速度 (忽略 Y 轴，防止跳跃/下落影响跑步动画)
        // 如果这是第一人称或第三人称角色，通常我们只关心 XZ 平面的速度
        Vector3 horizontalVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
        
        // 2. 计算速度大小 (magnitude)
        _currentSpeed = horizontalVelocity.magnitude;



        // 4. 将速度赋值给 Animator
        // 使用 SetFloat 更新 "Speed" 参数
        _animator.SetFloat(SpeedHash, _currentSpeed);
    }

    // --- 调试与可视化 ---

    /// <summary>
    /// 在 Scene 视图中实时绘制速度向量
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawDebugGizmos || _characterController == null) return;

        // 绘制速度方向（青色）
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, _characterController.velocity * 0.1f); // 缩小倍数以便观察

        // 绘制水平速度方向（黄色）
        Vector3 horizontalVel = new Vector3(_characterController.velocity.x, 0, _characterController.velocity.z);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, horizontalVel * 0.1f);
    }
}

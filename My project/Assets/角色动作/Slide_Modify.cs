using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
    [Header("胶囊体设置")]
    public CharacterController controller; // 或者使用 CapsuleCollider
    public Animator animator; // 角色的Animator

    [Header("尺寸参数")]
    public float standHeight = 1.7f;
    public float slideHeight = 0.6f; // 滑铲时的高度
    public float slideCenterY = 0.3f; // 滑铲时的中心点高度

    [Header("过渡速度")]
    public float smoothSpeed = 10f;

    private bool isSliding = false;
    private Vector3 targetCenter;
    private float targetHeight;
    private int slideHash; // 用于存储滑铲动画的哈希值

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponent<Animator>();

        targetHeight = standHeight;
        targetCenter = new Vector3(0, standHeight / 2, 0);

        // 获取滑铲动画的哈希值，确保正确监听动画
        slideHash = Animator.StringToHash("Slide_Fwd"); // 假设动画状态的名字是 "Slide_Fwd"
    }

    void Update()
    {
        // 检查是否进入滑铲动画状态
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slide_Fwd"))
        {
            // 如果角色进入滑铲状态
            if (!isSliding)
            {
                // 设置胶囊体的目标值
                targetHeight = slideHeight;
                targetCenter = new Vector3(0, slideCenterY, 0);
                isSliding = true;
            }
        }
        else
        {
            // 如果角色不在滑铲状态，恢复默认设置
            if (isSliding)
            {
                targetHeight = standHeight;
                targetCenter = new Vector3(0, standHeight / 2, 0);
                isSliding = false;
            }
        }

        // 2. 平滑插值更新碰撞体
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * smoothSpeed);
        controller.center = Vector3.Lerp(controller.center, targetCenter, Time.deltaTime * smoothSpeed);
    }
}
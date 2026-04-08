using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
    [Header("胶囊体设置")]
    public CharacterController controller; // 或者使用 CapsuleCollider
    public Transform meshTransform; // 角色的模型（用于辅助判断，可选）

    [Header("尺寸参数")]
    public float standHeight = 1.7f;
    public float slideHeight = 0.6f; // 滑铲时的高度
    public float slideCenterY = 0.3f; // 滑铲时的中心点高度

    [Header("过渡速度")]
    public float smoothSpeed = 10f;

    private bool isSliding = false;
    private Vector3 targetCenter;
    private float targetHeight;

    private float slideDuration = 0.6f; // 滑铲持续时间（例如1秒后恢复）
    private Coroutine slideCoroutine;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        targetHeight = standHeight;
        targetCenter = new Vector3(0, standHeight / 2, 0);
    }

    void Update()
    {
        // 1. 判断输入或状态 (这里假设按 C 键滑铲)
        if (Input.GetKeyDown(KeyCode.C) && !isSliding)
        {
            isSliding = true;
            // 设置目标状态为滑铲
            targetHeight = slideHeight;
            targetCenter = new Vector3(0, slideCenterY, 0);

            // 开始一个协程来恢复胶囊体状态
            if (slideCoroutine != null) 
                StopCoroutine(slideCoroutine); // 如果有正在执行的协程，停止它

            slideCoroutine = StartCoroutine(SlideCoroutine());
        }

        // 2. 平滑插值更新碰撞体
        // 注意：CharacterController 的 center 是相对于自身坐标系的
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * smoothSpeed);
        controller.center = Vector3.Lerp(controller.center, targetCenter, Time.deltaTime * smoothSpeed);
    }

    // 恢复到原状态的协程
    private IEnumerator SlideCoroutine()
    {
        // 等待滑铲持续时间
        yield return new WaitForSeconds(slideDuration);

        // 滑铲结束，恢复到站立状态
        targetHeight = standHeight;
        targetCenter = new Vector3(0, standHeight / 2, 0);
        isSliding = false; // 结束滑铲状态
    }
}
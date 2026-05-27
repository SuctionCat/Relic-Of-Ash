using UnityEngine;

public class Camera_move : MonoBehaviour
{
    public float moveSpeed = 5f;

    // 摄像机最大偏移
    public Vector2 maxOffset = new Vector2(2f, 1.5f);

    private Vector3 originalPosition;
    private Vector3 targetPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        // 鼠标位置归一化到 [-1,1]
        float normalizedX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float normalizedY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // 根据比例计算偏移
        float offsetX = normalizedX * maxOffset.x;
        float offsetY = normalizedY * maxOffset.y;

        // 目标位置
        targetPosition = originalPosition
                       + transform.right * offsetX
                       + transform.up * offsetY;

        // 平滑移动
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }
}
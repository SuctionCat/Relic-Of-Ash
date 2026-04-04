using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Settings")]
    public float mouseSensitivity = 200f;
    public float distance = 5f;
    public float height = 1.5f;
    public float smoothSpeed = 10f;

    [Header("Clamp")]
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public Transform cam;

    [Header("Custom Reference")]
    public Transform customPitchReference;  // 自定义上下旋转参考对象

    private float yaw;
    private float pitch = 20f;

    void Start()
    {
        if (cam == null)
            cam = GetComponentInChildren<Camera>().transform;

        if (customPitchReference == null)
        {
            // 如果没有指定自定义参考对象，就使用摄像机本身的pivot
            customPitchReference = transform;
        }

        // 初始化角度
        yaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 直接调用自由视角
        HandleFreeCamera();
    }

    // =========================
    // 🟢 自由视角
    // =========================
    void HandleFreeCamera()
    {
        // 获取鼠标输入
    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

    // 更新 yaw 和 pitch
    yaw += mouseX;  // 水平旋转（围绕角色）
    pitch -= mouseY;  // 垂直旋转（围绕自定义参照对象）

    // 限制上下旋转范围
    pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

    // 水平旋转：角色旋转
    transform.rotation = Quaternion.Euler(0, yaw, 0);

    // 垂直旋转：由自定义参照对象控制摄像机的上下旋转
    customPitchReference.localRotation = Quaternion.Euler(pitch, 0, 0);

    // 相机的位置平滑过渡
    Vector3 targetPos = player.position + Vector3.up * height;
    transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

    // 防止相机穿透角色
    Vector3 cameraPos = cam.position;
    RaycastHit hit;
    Vector3 dir = (cameraPos - player.position).normalized;
    if (Physics.Raycast(player.position, dir, out hit, distance))
    {
        cameraPos = hit.point; // 如果有障碍物，调整相机位置
    }

    // 设置摄像机相对于目标的距离
    Camera.main.transform.position = cameraPos;
    Camera.main.transform.localPosition = new Vector3(1.5f, 0, -distance);
    }
}
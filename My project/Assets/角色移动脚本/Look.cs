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

    // --- 🟢 索敌系统变量 ---
    private bool isTargeting = false;       // 是否开启索敌模式
    public Transform currentTarget;         // 当前锁定的目标
    public float targetingSmoothSpeed = 5f; // 索敌时的平滑速度

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

        // 🟢 根据索敌状态调用不同的视角处理函数
        if (isTargeting && currentTarget != null)
        {
            HandleTargetingCamera();
        }
        else
        {
            HandleFreeCamera();
        }
    }

    // =========================
    // 🟢 索敌视角
    // =========================
    void HandleTargetingCamera()
    {
        // 获取鼠标输入，用于微调视角
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 更新 yaw 和 pitch (鼠标控制仍然有效)
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 计算指向敌人的方向
        Vector3 directionToEnemy = currentTarget.position - player.position;
        directionToEnemy.y = 0; // 忽略高度差

        if (directionToEnemy.sqrMagnitude > 0.1f) // 防止除零或抖动
        {
            // 计算理想的朝向角度
            float targetYaw = Quaternion.LookRotation(directionToEnemy).eulerAngles.y;

            // 平滑过渡到理想角度
            // 使用 Mathf.MoveTowardsAngle 处理角度环绕问题 (例如从359度到1度)
            yaw = Mathf.MoveTowardsAngle(yaw, targetYaw, targetingSmoothSpeed * Time.deltaTime);
        }

        // 应用旋转
        transform.rotation = Quaternion.Euler(0, yaw, 0);
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
            cameraPos = hit.point;
        }

        // 设置摄像机相对于目标的距离
        Camera.main.transform.position = cameraPos;
        Camera.main.transform.localPosition = new Vector3(1.5f, 0, -distance);
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
        yaw += mouseX;
        pitch -= mouseY;
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
            cameraPos = hit.point;
        }

        // 设置摄像机相对于目标的距离
        Camera.main.transform.position = cameraPos;
        Camera.main.transform.localPosition = new Vector3(1.5f, 0, -distance);
    }

    // 🟢 公开方法，供 ThirdPersonController 调用以更新索敌状态
    public void SetTargeting(bool targeting, Transform target)
    {
        isTargeting = targeting;
        currentTarget = target;
    }
}
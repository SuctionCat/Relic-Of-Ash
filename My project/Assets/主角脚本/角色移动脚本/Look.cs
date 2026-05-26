using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Settings")]
    public float mouseSensitivity = 200f;
    public float height = 1.5f;
    public float smoothSpeed = 10f;
    public float positionDeadZone = 0.1f;   // 位置死区，容忍微小抖动的范围（米）

    [Header("Clamp")]
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public Transform cam;

    [Header("Camera Offset")]
    public Vector3 cameraOffset = new Vector3(1.5f, 0, -5f);  // 摄像机相对于pivot的偏移（支持偏左/偏右）

    [Header("Collision")]
    public LayerMask collisionMask = ~0;   // 射线检测层（建议排除Player层）
    public float collisionOffset = 0.3f;   // 碰撞后沿法线外推的距离
    public float collisionSmoothSpeed = 15f; // 碰撞过渡速度

    // --- 🟢 索敌系统变量 ---
    private bool isTargeting = false;       // 是否开启索敌模式
    public Transform currentTarget;         // 当前锁定的目标
    public float targetingSmoothSpeed = 5f; // 索敌时的平滑速度

    private float yaw;
    private float pitch = 20f;
    private Vector3 _lastValidPosition;   // 缓存上一次确认跟随的玩家位置

    void Start()
    {
        if (cam == null)
            cam = GetComponentInChildren<Camera>().transform;

        // 初始化角度
        yaw = transform.eulerAngles.y;

        // 初始化缓存位置
        if (player != null)
            _lastValidPosition = player.position;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

        // 应用旋转：单一pivot同时控制yaw和pitch
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // 相机的位置平滑过渡（pivot跟随玩家），带死区处理
        Vector3 currentPlayerPos = player.position;
        float distanceToLastValid = Vector3.Distance(currentPlayerPos, _lastValidPosition);

        if (distanceToLastValid > positionDeadZone)
        {
            _lastValidPosition = currentPlayerPos;
        }

        Vector3 targetPos = _lastValidPosition + Vector3.up * height;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        // 防止相机穿透障碍物
        Vector3 pivotPos = transform.position;
        Vector3 desiredWorldPos = transform.TransformPoint(cameraOffset);
        Vector3 dir = desiredWorldPos - pivotPos;
        float maxDist = dir.magnitude;

        if (Physics.Raycast(pivotPos, dir.normalized, out RaycastHit hit, maxDist, collisionMask))
        {
            Vector3 safePos = hit.point + hit.normal * collisionOffset;
            cam.position = Vector3.Lerp(cam.position, safePos, collisionSmoothSpeed * Time.deltaTime);
        }
        else
        {
            cam.localPosition = Vector3.Lerp(cam.localPosition, cameraOffset, collisionSmoothSpeed * Time.deltaTime);
        }
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

        // 应用旋转：单一pivot同时控制yaw和pitch
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // 相机的位置平滑过渡（pivot跟随玩家），带死区处理
        Vector3 currentPlayerPos = player.position;
        float distanceToLastValid = Vector3.Distance(currentPlayerPos, _lastValidPosition);

        if (distanceToLastValid > positionDeadZone)
        {
            _lastValidPosition = currentPlayerPos;
        }

        Vector3 targetPos = _lastValidPosition + Vector3.up * height;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        // 防止相机穿透障碍物
        Vector3 pivotPos = transform.position;
        Vector3 desiredWorldPos = transform.TransformPoint(cameraOffset);
        Vector3 dir = desiredWorldPos - pivotPos;
        float maxDist = dir.magnitude;

        if (Physics.Raycast(pivotPos, dir.normalized, out RaycastHit hit, maxDist, collisionMask))
        {
            Vector3 safePos = hit.point + hit.normal * collisionOffset;
            cam.position = Vector3.Lerp(cam.position, safePos, collisionSmoothSpeed * Time.deltaTime);
        }
        else
        {
            cam.localPosition = Vector3.Lerp(cam.localPosition, cameraOffset, collisionSmoothSpeed * Time.deltaTime);
        }
    }

    // 🟢 公开方法，供 ThirdPersonController 调用以更新索敌状态
    public void SetTargeting(bool targeting, Transform target)
    {
        isTargeting = targeting;
        currentTarget = target;
    }
}
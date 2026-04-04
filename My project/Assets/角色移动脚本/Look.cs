using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 玩家角色的Transform
    public float rotationSpeed = 3f; // 鼠标旋转速度
    public float zoomSpeed = 2f; // 鼠标滚轮的缩放速度
    public float minZoom = 3f; // 最小缩放距离
    public float maxZoom = 10f; // 最大缩放距离

    private Camera mainCamera; // 主摄像机
    private float currentZoom = 5f; // 当前缩放距离
    private float currentYaw = 0f; // 水平旋转角度
    private float currentPitch = 20f; // 垂直旋转角度

    void Start()
    {
        // 获取场景中的主摄像机
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 控制相机旋转
        HandleRotation();

        // 控制相机缩放
        HandleZoom();
    }

    void LateUpdate()
    {
        // 确保摄像机始终跟随玩家
        Vector3 desiredPosition = player.position;
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // 根据鼠标的控制，调整摄像机的视角
        mainCamera.transform.position = desiredPosition - rotation * Vector3.forward * currentZoom;
        mainCamera.transform.rotation = rotation;
    }

    void HandleRotation()
    {
        // 获取鼠标移动的输入
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        // 更新当前的旋转角度
        currentYaw += horizontalInput * rotationSpeed;
        currentPitch -= verticalInput * rotationSpeed;

        // 限制垂直旋转角度，防止相机翻转
        currentPitch = Mathf.Clamp(currentPitch, 10f, 50f);
    }

    void HandleZoom()
    {
        // 获取鼠标滚轮的输入来进行缩放
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // 更新当前的缩放值
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }
}
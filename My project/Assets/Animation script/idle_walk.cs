using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class idle_walk : MonoBehaviour
{
    private Animator animator;
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        // 获取挂载在同一物体上的 Animator 组件
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 2. 获取玩家的输入 (WASD 或 方向键)
        float h = Input.GetAxis("Horizontal"); // 水平输入 (-1 到 1)
        float v = Input.GetAxis("Vertical");   // 垂直输入 (-1 到 1)

        // 3. 计算移动向量
        Vector3 movement = new Vector3(h, 0, v);

        // 4. 计算速度的大小 (magnitude)
        // 如果不动，值为0；如果移动，值在0到1之间（取决于是否归一化）
        speed = movement.magnitude;

        // 5. 将速度值传递给动画控制器中的 "speed" 参数
        animator.SetFloat("speed", speed);
    }
}

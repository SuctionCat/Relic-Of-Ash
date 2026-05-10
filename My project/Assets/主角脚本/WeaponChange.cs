using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("引用")]
    public Animator animator; // 拖入你的 Animator
    public int totalWeapons = 3; // 武器总数

    [Header("状态")]
    public int currentWeaponIndex = 0; // 当前武器索引 (0, 1, 2)

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        
        // 初始化：告诉动画器当前是第几把武器
        animator.SetInteger("WeaponIndex", currentWeaponIndex);
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
        {
            // 滚轮向上：索引 - 1 (切上一把)
            ChangeWeaponIndex(-1);
        }
        else if (scrollInput < 0f)
        {
            // 滚轮向下：索引 + 1 (切下一把)
            ChangeWeaponIndex(1);
        }
    }

    void ChangeWeaponIndex(int direction)
    {
        // 1. 计算新索引
        currentWeaponIndex += direction;

        // 2. 处理循环逻辑 (0-1-2-0-1-2...)
        if (currentWeaponIndex >= totalWeapons)
        {
            currentWeaponIndex = 0; // 超过最大值回到 0
        }
        else if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = totalWeapons - 1; // 小于 0 回到最大值
        }

        // 3. 将新索引发送给 Animator
        animator.SetInteger("WeaponIndex", currentWeaponIndex);
    }
}
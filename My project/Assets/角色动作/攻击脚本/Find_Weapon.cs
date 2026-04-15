using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Find_Weapon : MonoBehaviour
{
    // 变量类型改为 BaseWeapon，这样它可以接收 Sword, Katana 或 Stick
    private BaseWeapon currentWeapon;

    public void SetAttackActive(int isActive)
    {
        if (currentWeapon == null || !currentWeapon.gameObject.activeSelf)
        {
            FindActiveWeapon();
        }

        if (currentWeapon != null)
        {
            currentWeapon.SetAttackActive(isActive);
        }
    }

    private void FindActiveWeapon()
    {
        // 在子物体中查找所有继承自 BaseWeapon 的组件
        BaseWeapon[] allWeapons = GetComponentsInChildren<BaseWeapon>(true); // true 表示包含非激活物体

        foreach (BaseWeapon weapon in allWeapons)
        {
            // 找到那个处于激活状态（SetActive(true)）的武器
            if (weapon.gameObject.activeSelf)
            {
                currentWeapon = weapon;
                return; // 找到了就退出循环
            }
        }

        // 如果循环结束还没找到，说明手里没拿武器
        currentWeapon = null;
    }
}

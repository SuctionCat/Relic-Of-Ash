using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // 在Inspector面板中拖拽赋值
    [Header("武器引用")]
    public GameObject SwordInHand;  // 手上的刀
    public GameObject SwordOnWaist; // 腰间的刀
    public GameObject KatanaInRight_Hand; // 右手上的武士刀
    public GameObject Katana_Sheath_InLeft_Hand; // 武士刀鞘
    public GameObject Katana_Waist; // 腰间的武士刀
    public GameObject Katana_Sheath_Waist; // 腰间的武士刀鞘
    public GameObject Stick; //手上的刺剑
    public GameObject Stick_Waist; //腰间的刺剑
    

    void Start()
    {
        // 初始化武器状态
        SwordInHand.SetActive(true);
        SwordOnWaist.SetActive(false);
        KatanaInRight_Hand.SetActive(false);
        Katana_Sheath_InLeft_Hand.SetActive(false);
        Katana_Waist.SetActive(true);
        Katana_Sheath_Waist.SetActive(true);
        Stick.SetActive(false);
        Stick_Waist.SetActive(true);
    }

    public void Move_Sword_To_Hand() //切换武器时使用
    {
        SwordInHand.SetActive(true);  // 显示手上的刀
        SwordOnWaist.SetActive(false);  // 显示腰间的刀
    }
    public void Move_Sword_To_Waist() //切换武器时使用
    {
        SwordInHand.SetActive(false);   // 隐藏手上的刀
        SwordOnWaist.SetActive(true); // 显示腰间的刀
    }

    public void Move_Katana_To_Hand() //切换武器时使用
    {
        KatanaInRight_Hand.SetActive(true); // 显示右手上的武士刀
        Katana_Sheath_InLeft_Hand.SetActive(true); // 显示左手上的武士刀鞘
        Katana_Waist.SetActive(false); // 隐藏腰间的武士刀
        Katana_Sheath_Waist.SetActive(false); // 隐藏腰间的武士刀鞘
    }
    public void Move_Katana_To_Waist() //切换武器时使用
    {
        KatanaInRight_Hand.SetActive(false); // 隐藏右手上的武士刀
        Katana_Sheath_InLeft_Hand.SetActive(false); // 隐藏左手上的武士刀鞘 
        Katana_Waist.SetActive(true); // 显示腰间的武士刀
        Katana_Sheath_Waist.SetActive(true); // 显示腰间的武士刀鞘
    }
    public void Move_Stick_To_Hand() //切换武器时使用
    {
        Stick.SetActive(true); // 显示手上的刺剑
        Stick_Waist.SetActive(false); // 隐藏腰间的刺剑
    }
    public void Move_Stick_To_Waist() //切换武器时使用
    {
        Stick.SetActive(false); // 隐藏手上的刺剑
        Stick_Waist.SetActive(true); // 显示腰间的刺剑
    }
}
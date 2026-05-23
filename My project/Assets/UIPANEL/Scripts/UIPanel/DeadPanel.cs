using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadPanel : BasePanel
{
    private static string Name = "DeadPanel";
    private static string Path = "Panel/DeadPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path, Name);
    
    private Button giveUpButton;
    private Button respawnButton;

    public DeadPanel() : base(UIPanelType) { }

    public override void ONStart()
    {
        base.ONStart();
        
        giveUpButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj, "GiveUp");
        respawnButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj, "Respawn");
        
        if(giveUpButton != null)
        {
            giveUpButton.onClick.AddListener(GiveUpButtonClick);
            Debug.Log("DeadPanel: 放弃按钮已绑定");
        }
        else
        {
            Debug.LogError("DeadPanel: 放弃按钮未找到！");
        }
        
        if(respawnButton != null)
        {
            respawnButton.onClick.AddListener(RespawnButtonClick);
            Debug.Log("DeadPanel: 重生按钮已绑定");
        }
        else
        {
            Debug.LogError("DeadPanel: 重生按钮未找到！");
        }
        
        GameRoot.GetInstance().EnterProtectMode();
        Debug.Log("DeadPanel: 已进入保护模式");
    }

    private void GiveUpButtonClick()
    {
        AudioManager.PlayClick();
        Debug.Log("DeadPanel: 点击放弃按钮");
        
        GameRoot.GetInstance().ExitProtectMode();
        GameRoot.GetInstance().UIManager_Root.Pop(true);
        
        Scene1 scene1 = new Scene1();
        GameRoot.GetInstance().ScenesControl_Root.LoadScene(scene1.SceneName, scene1);
    }

    private void RespawnButtonClick()
    {
        AudioManager.PlayClick();
        // 重生代码区域
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerRespawn();
            Debug.Log("调用玩家重生");
            
            // 退出保护模式（恢复游戏时间和锁定光标）
            GameRoot.GetInstance().ExitProtectMode();
            // 死亡面板出栈
            GameRoot.GetInstance().UIManager_Root.Pop(false);
        }
        else
        {
            Debug.LogWarning("GameManager.Instance 为空，无法触发重生");
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        GameRoot.GetInstance().ExitProtectMode();
    }
}

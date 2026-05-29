using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : BasePanel
{
    private static string Name = "GameOverPanel";
    private static string Path = "Panel/GameOverPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path, Name);

    public GameOverPanel() : base(UIPanelType) { }

    public override void ONStart()
    {
        base.ONStart();
        
        GameRoot.GetInstance().EnterProtectMode();
        GameRoot.GetInstance().RegisterUpdateMethod(HandleFKeyInput);
        
        Debug.Log("GameOverPanel ONStart called");
    }

    private void HandleFKeyInput()
    {
        if (UIManager.GetInstance().stack_ui.Count == 0 ||
            UIManager.GetInstance().stack_ui.Peek() != this)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ClosePanel();
        }
    }

    private void ClosePanel()
    {
        AudioManager.PlayClick();
        Debug.Log("Pressed F key, closing GameOverPanel...");
        GameRoot.GetInstance().ExitProtectMode();
        GameRoot.GetInstance().UIManager_Root.Pop(false);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        UIManager.GetInstance().DisableAllPanelsExcept(this);
        GameRoot.GetInstance().RegisterUpdateMethod(HandleFKeyInput);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        GameRoot.GetInstance().UnregisterUpdateMethod(HandleFKeyInput);
        UIManager.GetInstance().RestorePreviousPanelInteractivity();
    }

    public override void OnDestroy()
    {
        GameRoot.GetInstance().UnregisterUpdateMethod(HandleFKeyInput);
        GameRoot.GetInstance().ExitProtectMode();
        base.OnDestroy();
    }
}

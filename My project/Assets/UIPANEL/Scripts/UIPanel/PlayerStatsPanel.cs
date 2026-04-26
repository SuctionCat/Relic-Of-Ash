using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsPanel : BasePanel
{
    private static string Name="PlayerStatsPanel";
    private static string Path="Panel/PlayerStatsPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);

public PlayerStatsPanel():base(UIPanelType)
{
    // uiType is already set in the base constructor
}
    // Start is called before the first frame update
    public override void ONStart()
    {
        base.ONStart();
        UImched.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Back").onClick.AddListener(BackButtonClick);
    }
    private void BackButtonClick()
    {
        // 播放点击音效
        AudioManager.PlayClick();
        GameRoot.GetInstance().UIManager_Root.Pop(false);
    }
    public override void OnEnable()
    {
        base.OnEnable();
        UIManager.GetInstance().DisableAllPanelsExcept(this);
    }
    public override void OnDisable()
    {
        Debug.Log("PlayerStatsPanel IS back");
        base.OnDisable();
        UIManager.GetInstance().RestorePreviousPanelInteractivity();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
   
}
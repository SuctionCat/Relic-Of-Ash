using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    private static string Name="StartPanel";
    private static string Path="Panel/StartPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);
public StartPanel():base(UIPanelType)
{
    // uiType is already set in the base constructor
}
    public override void ONStart()
    {
        base.ONStart();
        UImched.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Back").onClick.AddListener(BackButtonClick);
        UImched.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Load").onClick.AddListener(LoadButtonClick);
        UImched.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Setting").onClick.AddListener(SettingButtonClick);
    }
    private void SettingButtonClick()
    {   
        GameRoot.GetInstance().UIManager_Root.Push(new SettingPanel());
    }
    private void BackButtonClick()
    {
        GameRoot.GetInstance().UIManager_Root.Pop(false);
    }
    private void LoadButtonClick()
    {
        Scene2 scene2 = new Scene2();
        GameRoot.GetInstance().ScenesControl_Root.LoadScene(scene2.SceneName,scene2);
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnDisable()
    {
        Debug.Log("StartPanel IS back");
        base.OnDisable();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
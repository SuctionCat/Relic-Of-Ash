using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    private static string Name="StartPanel";
    private static string Path="Panel/StartPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);
    
    // 按钮引用
    private Button backButton;
    private Button loadButton;
    private Button settingButton;

public StartPanel():base(UIPanelType)
{
}

    public override void ONStart()
    {
        base.ONStart();
        
        // 获取按钮引用
        backButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Back");
        loadButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Load");
        settingButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Setting");
        
        // 添加按钮点击事件
        backButton.onClick.AddListener(BackButtonClick);
        loadButton.onClick.AddListener(LoadButtonClick);
        settingButton.onClick.AddListener(SettingButtonClick);
    }

    private void SettingButtonClick()
    {
        AudioManager.PlayClick();
        GameRoot.GetInstance().UIManager_Root.Push(new SettingPanel());
    }

    private void BackButtonClick()
    {
        AudioManager.PlayClick();
        GameRoot.GetInstance().UIManager_Root.Push(new QuitPanel());
    }

    private void LoadButtonClick()
    {
        AudioManager.PlayClick();
        PlayScene playScene = new PlayScene();
        GameRoot.GetInstance().ScenesControl_Root.LoadSceneAsync(playScene.SceneName, playScene);
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

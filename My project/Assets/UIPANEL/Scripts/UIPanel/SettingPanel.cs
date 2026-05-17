using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    private static string Name="SettingPanel";
    private static string Path="Panel/SettingPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);

    private Slider musicSlider;
    private Slider sfxSlider;

public SettingPanel():base(UIPanelType)
{
    // uiType is already set in the base constructor
}
    // Start is called before the first frame update
    public override void ONStart()
    {
        base.ONStart();
        UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Back").onClick.AddListener(BackButtonClick);
        
        // 初始化音量滑块
        musicSlider = UImchud.GetInstance().GetOrAddComponent<Slider>(ActiveObj,"MusicSlider");
        sfxSlider = UImchud.GetInstance().GetOrAddComponent<Slider>(ActiveObj,"SFXSlider");
        
        if(musicSlider != null)
        {
            // 加载保存的音乐音量，默认为1.0
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100.0f);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        if(sfxSlider != null)
        {
            // 加载保存的音效音量，默认为1.0
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 100.0f);
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        // 进入保护模式
        GameRoot.GetInstance().EnterProtectMode();
    }
    
    private void OnMusicVolumeChanged(float value)
    {
        // 保存音乐音量
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        
        // 应用音乐音量
        // 这里需要根据你的音频管理系统来设置音量
        // 例如：AudioManager.Instance.SetMusicVolume(value);
        Debug.Log($"音乐音量设置为: {value}");
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        // 保存音效音量
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        
        // 应用音效音量
        // 这里需要根据你的音频管理系统来设置音量
        // 例如：AudioManager.Instance.SetSFXVolume(value);
        Debug.Log($"音效音量设置为: {value}");
    }
    
    private void BackButtonClick()
    {
        // 播放点击音效
        AudioManager.PlayClick();
        GameRoot.GetInstance().ExitProtectMode();
        GameRoot.GetInstance().UIManager_Root.Pop(false);
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        UIManager.GetInstance().DisableAllPanelsExcept(this);
    }
    
    public override void OnDisable()
    {
        Debug.Log("SettingPanel IS back");
        base.OnDisable();
        UIManager.GetInstance().RestorePreviousPanelInteractivity();
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
   
}
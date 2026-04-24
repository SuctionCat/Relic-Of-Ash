using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : BasePanel
{
    private static string Name="SettingPanel";
    private static string Path="Panel/SettingPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);
public SettingPanel():base(UIPanelType)
{
    // uiType is already set in the base constructor
}   
    // Start is called before the first frame update
    public override void ONStart()
    {
        base.ONStart();
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnDisable()
    {
        Debug.Log("SettingPanel IS back");
        base.OnDisable();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScene : ScenesBase
{
   public readonly string SceneName="MenuScene";
    public override void EnterScene()
    {
        // 设置当前场景名称
        GameRoot.GetInstance().SetCurrentSceneName(SceneName);
        
        // 在主菜单场景中，鼠标应该可见且未锁定
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // 加载 StartPanel
        GameRoot.GetInstance().UIManager_Root.Push(new StartPanel());
    }
    public override void ExitScene()
    {
        // 清除当前场景名称
        GameRoot.GetInstance().SetCurrentSceneName("");
    }

}
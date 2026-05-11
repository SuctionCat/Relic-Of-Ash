using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1 : ScenesBase
{
   public readonly string SceneName="Scene1";
    public override void EnterScene()
    {
        // 设置当前场景名称
        GameRoot.GetInstance().SetCurrentSceneName(SceneName);
        
        // 加载 StartPanel
        GameRoot.GetInstance().UIManager_Root.Push(new StartPanel());
    }
    public override void ExitScene()
    {
        // 清除当前场景名称
        GameRoot.GetInstance().SetCurrentSceneName("");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2 : ScenesBase
{
   public readonly string SceneName="Scene2";


     public override void EnterScene()
    {
        // 设置当前场景名称，使ESC键可以打开暂停菜单
        GameRoot.GetInstance().SetCurrentSceneName(SceneName);

        // 使用协程延迟加载Player Stats UI面板
        // 因为SceneManager.LoadScene后，新场景可能还没有完全加载完成
        GameRoot.GetInstance().StartCoroutine(LoadPlayerStatsPanelCoroutine());
    }

    private System.Collections.IEnumerator LoadPlayerStatsPanelCoroutine()
    {
        // 等待一帧，确保新场景完全加载完成
        yield return null;

        // 再等待一帧，确保GameRoot的Start方法已经执行
        yield return null;

        // 加载Player Stats UI面板
        Debug.Log("正在加载PlayerStatsPanel...");
        GameRoot.GetInstance().UIManager_Root.Push(new PlayerStatsPanel());
    }

    public override void ExitScene()
    {

    }

}

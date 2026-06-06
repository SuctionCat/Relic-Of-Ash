using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScene : ScenesBase
{
    public readonly string SceneName = "EndScene";

    public override void EnterScene()
    {
        GameRoot.GetInstance().SetCurrentSceneName(SceneName);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameRoot.GetInstance().StartCoroutine(LoadPlayerStatsPanelCoroutine());
    }

    public override void ExitScene()
    {
    }

    private System.Collections.IEnumerator LoadPlayerStatsPanelCoroutine()
    {
        yield return null;
        yield return null;

        Debug.Log("正在加载PlayerStatsPanel...");
        GameRoot.GetInstance().UIManager_Root.Push(new PlayerStatsPanel());
    }
}

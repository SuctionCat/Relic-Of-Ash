using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene3 : ScenesBase
{
    public readonly string SceneName = "Scene3";

    public override void EnterScene()
    {
        GameRoot.GetInstance().SetCurrentSceneName(SceneName);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameRoot.GetInstance().StartCoroutine(LoadPlayerStatsPanelCoroutine());
    }

    private System.Collections.IEnumerator LoadPlayerStatsPanelCoroutine()
    {
        yield return null;
        yield return null;

        Debug.Log("正在加载PlayerStatsPanel...");
        GameRoot.GetInstance().UIManager_Root.Push(new PlayerStatsPanel());
    }

    public override void ExitScene()
    {

    }
}

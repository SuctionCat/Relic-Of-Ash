using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitPanel : BasePanel
{
    private static string Name="QuitPanel";
    private static string Path="Panel/QuitPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);

    private Button quitButton;
    private Button backButton;

public QuitPanel():base(UIPanelType)
{
}

    public override void ONStart()
    {
        base.ONStart();

        // 获取按钮引用
        Button[] buttons = ActiveObj.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.interactable = true;
            
            if (btn.name == "Quit")
            {
                quitButton = btn;
                quitButton.onClick.AddListener(QuitButtonClick);
            }
            else if (btn.name == "Back")
            {
                backButton = btn;
                backButton.onClick.AddListener(BackButtonClick);
            }
        }
    }

    private void QuitButtonClick()
    {
        AudioManager.PlayClick();
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void BackButtonClick()
    {
        AudioManager.PlayClick();
        
        if (GameRoot.GetInstance()?.UIManager_Root != null)
        {
            GameRoot.GetInstance().UIManager_Root.Pop(false);
        }
        else if (ActiveObj != null)
        {
            ActiveObj.SetActive(false);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        // 确保面板可以交互
        CanvasGroup canvasGroup = ActiveObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = ActiveObj.AddComponent<CanvasGroup>();
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public override void OnDisable()
    {
        Debug.Log("QuitPanel IS back");
        base.OnDisable();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}

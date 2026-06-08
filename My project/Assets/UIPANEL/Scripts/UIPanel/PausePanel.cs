using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : BasePanel
{
    private static string Name = "PausePanel";
    private static string Path = "Panel/PausePanel";
    public static readonly UITepy UIPanelType = new UITepy(Path, Name);

    private Button continueButton;
    private Button mainMenuButton;
    private Button settingsButton;
    private Button breakDeadlockButton;
    
    private bool isPaused = false;
    private bool justOpened = false;

    public PausePanel() : base(UIPanelType) { }

    public override void ONStart()
    {
        base.ONStart();
        
        continueButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj, "Continue");
        mainMenuButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj, "MainMenu");
        settingsButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj, "Settings");
        breakDeadlockButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj, "BreakDeadlock");
        
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueButtonClick);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(MainMenuButtonClick);
        }
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(SettingsButtonClick);
        }
        if (breakDeadlockButton != null)
        {
            breakDeadlockButton.onClick.AddListener(BreakDeadlockButtonClick);
        }
        
        CanvasGroup canvasGroup = UImchud.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isPaused = true;
        justOpened = true;
        
        GameRoot.GetInstance().EnterProtectMode();
        GameRoot.GetInstance().RegisterUpdateMethod(HandleESCInput);
    }
    
    private void ContinueButtonClick()
    {
        AudioManager.PlayClick();
        ResumeGame();
    }
    
    private void MainMenuButtonClick()
    {
        AudioManager.PlayClick();
        ResumeGame();
        
        GameRoot.GetInstance().UIManager_Root.Pop(true);
        
        MenuScene menuScene = new MenuScene();
        GameRoot.GetInstance().ScenesControl_Root.LoadScene(menuScene.SceneName, menuScene);
    }
    
    private void SettingsButtonClick()
    {
        AudioManager.PlayClick();
        GameRoot.GetInstance().UIManager_Root.Push(new SettingPanel());
    }
    
    private void BreakDeadlockButtonClick()
    {
        AudioManager.PlayClick();
        ResumeGame();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TeleportToCheckpoint();
        }
        else
        {
            Debug.LogWarning("PausePanel: GameManager.Instance 为空，无法执行脱离卡死");
        }
    }
    
    public void ResumeGame()
    {
        if(isPaused)
        {
            isPaused = false;
            UIManager.GetInstance().RestorePreviousPanelInteractivity();
            GameRoot.GetInstance().ExitProtectMode();
            GameRoot.GetInstance().UIManager_Root.Pop(false);
        }
    }
    
    public void PauseGame()
    {
        if(!isPaused)
        {
            UIManager.GetInstance().DisableAllPanelsExcept(this);
            Time.timeScale = 0.0f;
            isPaused = true;
            GameRoot.GetInstance().RegisterUpdateMethod(HandleESCInput);
        }
    }
    
    private void HandleESCInput()
    {
        if (UIManager.GetInstance().stack_ui.Count == 0 ||
            UIManager.GetInstance().stack_ui.Peek() != this)
        {
            return;
        }

        if (justOpened)
        {
            justOpened = false;
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        isPaused = true;
        justOpened = true;
        GameRoot.GetInstance().RegisterUpdateMethod(HandleESCInput);
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        GameRoot.GetInstance().UnregisterUpdateMethod(HandleESCInput);
        
        if (UIManager.GetInstance() != null)
            UIManager.GetInstance().RestorePreviousPanelInteractivity();
    }
    
    public override void OnDestroy()
    {
        GameRoot.GetInstance().UnregisterUpdateMethod(HandleESCInput);
        
        if(isPaused)
        {
            GameRoot.GetInstance().ExitProtectMode();
        }
        
        base.OnDestroy();
    }
}
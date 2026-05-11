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
    
    private bool isPaused = false;
    private bool justOpened = false;

    public PausePanel() : base(UIPanelType) { }

    public override void ONStart()
    {
        base.ONStart();
        
        continueButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj, "Continue");
        mainMenuButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj, "MainMenu");
        settingsButton = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj, "Settings");
        
        if (continueButton != null)
        {
            continueButton.interactable = true;
            continueButton.onClick.AddListener(ContinueButtonClick);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.interactable = true;
            mainMenuButton.onClick.AddListener(MainMenuButtonClick);
        }
        if (settingsButton != null)
        {
            settingsButton.interactable = true;
            settingsButton.onClick.AddListener(SettingsButtonClick);
        }

        UIManager.GetInstance().DisableAllPanelsExcept(this);

        CanvasGroup canvasGroup = UImchud.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        Time.timeScale = 0.0f;
        
        isPaused = true;
        justOpened = true;
        
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
        
        Scene1 scene1 = new Scene1();
        GameRoot.GetInstance().ScenesControl_Root.LoadScene(scene1.SceneName, scene1);
    }
    
    private void SettingsButtonClick()
    {
        AudioManager.PlayClick();
        GameRoot.GetInstance().UIManager_Root.Push(new SettingPanel());
    }
    
    public void ResumeGame()
    {
        if(isPaused)
        {
            UIManager.GetInstance().RestorePreviousPanelInteractivity();
            Time.timeScale = 1.0f;
            GameRoot.GetInstance().UIManager_Root.Pop(false);
            isPaused = false;
            GameRoot.GetInstance().UnregisterUpdateMethod(HandleESCInput);
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
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        
        if (UIManager.GetInstance() != null)
            UIManager.GetInstance().RestorePreviousPanelInteractivity();
        
        Time.timeScale = 1.0f;
        isPaused = false;
    }
    
    public override void OnDestroy()
    {
        GameRoot.GetInstance().UnregisterUpdateMethod(HandleESCInput);
        
        if(Time.timeScale == 0.0f)
            Time.timeScale = 1.0f;
        
        base.OnDestroy();
    }
}
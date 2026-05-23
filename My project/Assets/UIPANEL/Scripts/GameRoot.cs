using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private UIManager UIManager;
    public UIManager UIManager_Root{get=> UIManager;}
    private ScenesControl ScenesControl;
    public ScenesControl ScenesControl_Root{get=> ScenesControl;}
    private static GameRoot instance;
    public static GameRoot GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("GameRoot 获得实例失败");
            return instance;
        }
        return instance;
    }

    // 当前场景名称
    private string currentSceneName = "";
    
    // 全局暂停状态
    private bool isGamePaused = false;
    public bool IsGamePaused { get => isGamePaused; set => isGamePaused = value; }
    
    // 保护面板计数
    private int protectPanelCount = 0;

    // 存储需要在Update中调用的方法
    private List<System.Action> updateMethods = new List<System.Action>();

    // 注册Update方法
    public void RegisterUpdateMethod(System.Action updateMethod)
    {
        if(!updateMethods.Contains(updateMethod))
        {
            updateMethods.Add(updateMethod);
        }
    }

    // 取消注册Update方法
    public void UnregisterUpdateMethod(System.Action updateMethod)
    {
        if(updateMethods.Contains(updateMethod))
        {
            updateMethods.Remove(updateMethod);
        }
    }

    // 更新当前场景名称
    public void SetCurrentSceneName(string sceneName)
    {
        currentSceneName = sceneName;
        Debug.Log($"当前场景设置为: {sceneName}");
    }
    
    // 进入保护模式
    public void EnterProtectMode()
    {
        protectPanelCount++;
        
        if(protectPanelCount == 1)
        {
            IsGamePaused = true;
            Time.timeScale = 0.0f;
            // 解锁鼠标并使其可见，以便操作UI
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    // 退出保护模式
    public void ExitProtectMode()
    {
        protectPanelCount--;
        
        if(protectPanelCount <= 0)
        {
            protectPanelCount = 0;
            IsGamePaused = false;
            Time.timeScale = 1.0f;
            // 根据当前场景决定是否锁定鼠标
            // 在主菜单场景(Scene1)中，鼠标应该保持可见
            // 在游戏场景(Scene2)中，鼠标应该被锁定
            if(currentSceneName == "Scene2")
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if(currentSceneName == "Scene1")
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    // 处理ESC键打开对应面板
    private void HandleESCKey()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentSceneName == "Scene2")
            {
                bool hasPausePanel = false;
                foreach(var panel in UIManager_Root.stack_ui)
                {
                    if(panel.uiType.Name == "PausePanel")
                    {
                        hasPausePanel = true;
                        break;
                    }
                }
                if(hasPausePanel)
                {
                    return;
                }
                Debug.Log("检测到ESC键，打开暂停菜单");
                UIManager_Root.Push(new PausePanel());
            }
            else if(currentSceneName == "Scene1")
            {
                Debug.Log("检测到ESC键，打开退出游戏面板");
                UIManager_Root.Push(new QuitPanel());
            }
        }
    }

    private void Update()
    {
        // 处理ESC键
        HandleESCKey();

        // 创建一个副本，避免在遍历过程中修改原集合
        List<System.Action> methodsCopy = new List<System.Action>(updateMethods);
        foreach(var method in methodsCopy)
        {
            method?.Invoke();
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        UIManager = UIManager.GetInstance();
        ScenesControl = ScenesControl.GetInstance();

    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        GameObject canvas = UImchud.GetInstance().FindCanvas();
        if (canvas != null)
        {
            UIManager_Root.CanvasObj = canvas;
        }
        else
        {
            Debug.LogError("Canvas object not found");
        }
        Scene1 scene1 = new Scene1();
        ScenesControl_Root.dict_scenes.Add(scene1.SceneName,scene1);
        #region
        UIManager_Root.Push(new InitialPanel());
        #endregion
    }

    private void HandlePlayerDead()
    {
        Debug.Log("GameRoot: 处理玩家死亡，3秒后显示死亡面板");
        StartCoroutine(ShowDeadPanelAfterDelay(3f));
    }

    private IEnumerator ShowDeadPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UIManager_Root.Push(new DeadPanel());
    }

    public void RegisterStateManager()
    {
        if (StateManager.instance != null)
        {
            StateManager.instance.OnPlayerDead += HandlePlayerDead;
            Debug.Log("GameRoot: StateManager 已注册，玩家死亡事件订阅成功");
        }
    }
}


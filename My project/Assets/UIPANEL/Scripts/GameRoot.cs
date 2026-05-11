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

    // 处理ESC键打开对应面板
    private void HandleESCKey()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentSceneName == "Scene2")
            {
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
        // Find Canvas object using UImchud
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
}

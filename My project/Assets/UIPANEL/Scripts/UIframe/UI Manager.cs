using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{ 
    /// <summary>
    /// 存储uiPanel的名称与物体的对应关系
    /// </summary>
    public Dictionary<string, GameObject> dict_uiObject;
    /// <summary>
    /// 存储栈的uiPanel的栈结构
    /// </summary>
    public BasePanel currentPanel;

    public UITepy currentPanelType;
    public Stack<BasePanel> stack_ui;

    /// <summary>
    /// 当前场景里的Canvas物体
    /// </summary>
    public GameObject CanvasObj;
    ///
    private static UIManager instance;
    public static UIManager GetInstance()
    {
        if(instance==null)
        {
            instance = new UIManager();
        }
        return instance;
    }

    private UIManager()
    {
        instance = this;
        dict_uiObject = new Dictionary<string, GameObject>();
        stack_ui = new Stack<BasePanel>();
    }

    /// <summary>
    /// 设置Canvas对象
    /// </summary>
    /// <param name="canvas">Canvas游戏对象</param>
    public void SetCanvas(GameObject canvas)
    {
        CanvasObj = canvas;
        Debug.Log("Canvas对象已设置");
    }

    /// <summary>
    /// 确保Canvas对象存在
    /// </summary>
    private void EnsureCanvasExists()
    {
        Debug.Log($"EnsureCanvasExists: CanvasObj当前状态={CanvasObj != null}");
        
        if(CanvasObj == null)
        {
            Debug.Log("EnsureCanvasExists: Canvas对象为空，尝试查找...");
            CanvasObj = UImchud.GetInstance().FindCanvas();
            if(CanvasObj == null)
            {
                Debug.LogError("EnsureCanvasExists: 无法找到Canvas对象！");
            }
            else
            {
                Debug.Log($"EnsureCanvasExists: 成功找到Canvas对象: {CanvasObj.name}, 场景={CanvasObj.scene.name}");
            }
        }
        else
        {
            Debug.Log($"EnsureCanvasExists: CanvasObj已存在: {CanvasObj.name}, 场景={CanvasObj.scene.name}");
        }
    }

    public GameObject GetSingleObject(UITepy uITepy)
    {
        try
        {
            Debug.Log($"GetSingleObject: ========== 开始加载 {uITepy.Name} ==========");
            Debug.Log($"GetSingleObject: 路径={uITepy.Path}, 名称={uITepy.Name}");
            
            // 检查字典是否初始化
            if(dict_uiObject == null)
            {
                Debug.LogError("GetSingleObject: dict_uiObject 为 null，重新初始化");
                dict_uiObject = new Dictionary<string, GameObject>();
            }
            Debug.Log($"GetSingleObject: dict_uiObject 状态正常，计数={dict_uiObject.Count}");
            
            // 检查字典中是否存在该对象
            if(dict_uiObject.ContainsKey(uITepy.Name))
            {
                GameObject existingObj = dict_uiObject[uITepy.Name];
                
                // 检查对象是否已被销毁
                if(existingObj == null)
                {
                    Debug.LogError($"GetSingleObject: {uITepy.Name} 在字典中但对象已被销毁，移除无效引用");
                    dict_uiObject.Remove(uITepy.Name);
                }
                else
                {
                    // 使用 CompareTag 代替直接访问 activeSelf，避免访问已销毁对象
                    Debug.Log($"GetSingleObject: {uITepy.Name} 已存在于字典中");
                    return existingObj;
                }
            }

            // 确保Canvas存在
            Debug.Log($"GetSingleObject: 步骤1 - 确保Canvas存在");
            EnsureCanvasExists();

            if(CanvasObj == null)
            {
                Debug.LogError("GetSingleObject: Canvas对象为空，无法加载UI");
                return null;
            }
            
            Debug.Log($"GetSingleObject: 步骤2 - CanvasObj={CanvasObj.name}, Canvas场景={CanvasObj.scene.name}, 当前场景={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

            Debug.Log($"GetSingleObject: 步骤3 - 开始加载资源: Resources/Panel/{uITepy.Name}");
            GameObject prefab = Resources.Load<GameObject>(uITepy.Path);
            if(prefab == null)
            {
                Debug.LogError($"GetSingleObject: 步骤3失败 - 未能加载UI资源: {uITepy.Path}");
                Debug.LogError($"GetSingleObject: 请检查 Resources/Panel/ 目录下是否存在 {uITepy.Name}.prefab");
                return null;
            }
            Debug.Log($"GetSingleObject: 步骤3成功 - 成功加载预制体: {prefab.name}");
            
            Debug.Log($"GetSingleObject: 步骤4 - 开始实例化对象");
            GameObject gameObject = GameObject.Instantiate<GameObject>(prefab, CanvasObj.transform);
            Debug.Log($"GetSingleObject: 步骤4成功 - 成功创建对象: {gameObject.name}, 父对象={gameObject.transform.parent?.name}, active={gameObject.activeSelf}");
            
            dict_uiObject.Add(uITepy.Name, gameObject);
            Debug.Log($"GetSingleObject: 步骤5 - 添加到字典完成");
            
            Debug.Log($"GetSingleObject: ========== 加载 {uITepy.Name} 完成 ==========");
            return gameObject;
        }
        catch(System.Exception e)
        {
            Debug.LogError($"GetSingleObject: 加载 {uITepy.Name} 时发生异常: {e.Message}");
            Debug.LogError($"GetSingleObject: 异常堆栈: {e.StackTrace}");
            return null;
        }
    }
    public void Push(BasePanel basePanel_push)
    {
       Debug.Log($"{basePanel_push.uiType.Name}入栈 - 开始");
       Debug.Log($"{basePanel_push.uiType.Name}入栈 - 栈大小: {stack_ui.Count}");
       
      if(stack_ui.Count > 0)
      {
        BasePanel topPanel = stack_ui.Peek();
        Debug.Log($"{basePanel_push.uiType.Name}入栈 - 禁用上一个面板: {topPanel.uiType.Name}");
        
        // 检查栈顶面板的对象是否已被销毁
        if(topPanel.ActiveObj == null)
        {
            Debug.LogError($"{basePanel_push.uiType.Name}入栈 - 栈顶面板 {topPanel.uiType.Name} 的对象已被销毁，从栈中移除");
            stack_ui.Pop();
        }
        else
        {
            try
            {
                topPanel.OnDisable();
                Debug.Log($"{basePanel_push.uiType.Name}入栈 - 禁用上一个面板完成");
            }
            catch(System.Exception e)
            {
                Debug.LogError($"{basePanel_push.uiType.Name}入栈 - 禁用上一个面板异常: {e.Message}");
            }
        }
      }
      
      Debug.Log($"{basePanel_push.uiType.Name}入栈 - 调用 GetSingleObject");
      GameObject ui_object = GetSingleObject(basePanel_push.uiType);
      
      if(ui_object == null)
      {
          Debug.LogError($"{basePanel_push.uiType.Name}入栈 - 未能创建UI对象");
          return;
      }
      
      Debug.Log($"{basePanel_push.uiType.Name}入栈 - UI对象创建成功");
      
      if(!dict_uiObject.ContainsKey(basePanel_push.uiType.Name))
      {
          dict_uiObject.Add(basePanel_push.uiType.Name, ui_object);
          Debug.Log($"{basePanel_push.uiType.Name}入栈 - 添加到字典");
      }
      
      basePanel_push.ActiveObj = ui_object;
      Debug.Log($"{basePanel_push.uiType.Name}入栈 - 设置ActiveObj");
      
      if(stack_ui.Count==0)
      {
        stack_ui.Push(basePanel_push);
        Debug.Log($"{basePanel_push.uiType.Name}入栈 - 压入空栈");
      }
      else
      {
        if(stack_ui.Peek().uiType.Name!=basePanel_push.uiType.Name)
        {
          stack_ui.Push(basePanel_push);
          Debug.Log($"{basePanel_push.uiType.Name}入栈 - 压入非空栈");
        }
        else
        {
          Debug.Log($"{basePanel_push.uiType.Name}入栈 - 已存在，不重复压入");
        }
      }
      
      Debug.Log($"{basePanel_push.uiType.Name}入栈 - 调用 ONStart");
      try
      {
          basePanel_push.ONStart();
          Debug.Log($"{basePanel_push.uiType.Name}入栈 - ONStart 完成");
      }
      catch(System.Exception e)
      {
          Debug.LogError($"{basePanel_push.uiType.Name}入栈 - ONStart 异常: {e.Message}");
      }
      
      Debug.Log($"{basePanel_push.uiType.Name}入栈 - 完成");
    }
    //<summary>
    ///禁用除指定面板外的所有面板
    /// </summary>
    public void DisableAllPanelsExcept(BasePanel exceptPanel)
    {
        foreach(BasePanel panel in stack_ui)
        {
            CanvasGroup canvasGroup = UImchud.GetInstance().GetOrAddComponent<CanvasGroup>(panel.ActiveObj);
            if(panel != exceptPanel)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }

    //<summary>
    ///恢复上一个面板的交互性
    /// </summary>
    public void RestorePreviousPanelInteractivity()
    {
        if(stack_ui.Count > 0)
        {
            BasePanel previousPanel = stack_ui.Peek();
            UImchud.GetInstance().GetOrAddComponent<CanvasGroup>(previousPanel.ActiveObj).interactable = true;
        }
    }

    //<summary>
    ///出栈
    /// </summary>
    public void Pop(bool isload)
    {
        Debug.Log($"UIManager.Pop: isload={isload}, 当前栈大小={stack_ui.Count}");
        
        if(isload == true)
        {
            int initialCount = stack_ui.Count;
            Debug.Log($"UIManager.Pop(true): 开始清空栈，当前栈大小={initialCount}");
            
            while(stack_ui.Count>0)
            {
                BasePanel topPanel = stack_ui.Peek();
                string panelName = topPanel.uiType.Name;
                Debug.Log($"UIManager.Pop(true): 正在销毁面板: {panelName}");
                
                topPanel.OnDisable();
                topPanel.OnDestroy();
                
                if(dict_uiObject.ContainsKey(panelName))
                {
                    GameObject.Destroy(dict_uiObject[panelName]);
                    dict_uiObject.Remove(panelName);
                    Debug.Log($"UIManager.Pop(true): 销毁成功，字典已移除: {panelName}");
                }
                stack_ui.Pop();
                Debug.Log($"UIManager.Pop(true): 弹出完成，剩余栈大小={stack_ui.Count}");
            }
            
            Debug.Log($"UIManager.Pop(true): 清空完成，共销毁 {initialCount} 个面板");
        }
        else
        {
            if(stack_ui.Count>0)
            {
                BasePanel topPanel = stack_ui.Peek();
                topPanel.OnDisable();
                topPanel.OnDestroy();
                string panelName = topPanel.uiType.Name;
                if(dict_uiObject.ContainsKey(panelName))
                {
                    GameObject.Destroy(dict_uiObject[panelName]);
                    dict_uiObject.Remove(panelName);
                }
                stack_ui.Pop();
                if(stack_ui.Count>0)
                {
                    currentPanel = stack_ui.Peek();
                    currentPanel.OnEnable();
                }
            }
        }
    }
}
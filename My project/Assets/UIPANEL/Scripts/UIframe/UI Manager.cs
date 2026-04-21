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
            Debug.Log("UIManager实例已创建");
        }
        else
        {
            Debug.Log("UIManager实例已存在");
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
    public GameObject GetSingleObject(UITepy uITepy)
    {
       if(dict_uiObject.ContainsKey(uITepy.Name))
       {
        return dict_uiObject[uITepy.Name];
       }
       if(CanvasObj==null)
       {
        CanvasObj = UImched.GetInstance().FindCanvas();

       } 
       GameObject prefab = Resources.Load<GameObject>(uITepy.Name);
       if(prefab == null)
       {
           Debug.Log($"未能加载UI资源: {uITepy.Name}");
           return null;
       }
       GameObject gameObject = GameObject.Instantiate<GameObject>(prefab, CanvasObj.transform);
       dict_uiObject.Add(uITepy.Name, gameObject);
       return gameObject;
    }
    public void Push(BasePanel basePanel_push)
    {
       Debug.Log($"{basePanel_push.uiType.Name}入栈");
      if(stack_ui.Count > 0)
      {
        stack_ui.Peek().OnDisable();
      }
      GameObject ui_object = GetSingleObject(basePanel_push.uiType);
      if(!dict_uiObject.ContainsKey(basePanel_push.uiType.Name))
      {
          dict_uiObject.Add(basePanel_push.uiType.Name, ui_object);
      }
      basePanel_push.ActiveObj = ui_object;
      if(ui_object == null)
      {
          Debug.Log($"未能创建UI对象: {basePanel_push.uiType.Name}");
          return;
      }
      if(stack_ui.Count==0)
      {
        stack_ui.Push(basePanel_push);
      }
      else
      {
        if(stack_ui.Peek().uiType.Name!=basePanel_push.uiType.Name)
        {
          stack_ui.Push(basePanel_push);
        }
      }
      basePanel_push.ONStart();
    }
    //<summary>
    ///出栈
    /// </summary>
    public void Pop(bool isload)
    {
        if(isload == true)
        {
            while(stack_ui.Count>0)
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
            }
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


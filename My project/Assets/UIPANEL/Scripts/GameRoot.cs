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
    
    private void Update()
    {
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
        // Find Canvas object using UImched
        GameObject canvas = UImched.GetInstance().FindCanvas();
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
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
    UIManager_Root.Push(new StartPanel());
    
    #endregion
    }
}
 
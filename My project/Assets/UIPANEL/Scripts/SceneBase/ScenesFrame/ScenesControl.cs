using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesControl 
{
    public Dictionary<string, ScenesBase> dict_scenes;
    private static ScenesControl instance;
    
    public ScenesControl()
    {
        dict_scenes = new Dictionary<string, ScenesBase>();
    }
    
    public static ScenesControl GetInstance()
    {
        if(instance == null)
        {
            instance = new ScenesControl();
            Debug.Log("ScenesControl实例已创建");
        }
        return instance;
    }
    /// <summary>
    /// 加载场景并进入场景
    /// </summary>
    /// <param name="scene_name">目标场景名称</param>
    /// <param name="sceneBase">场景基类</param>
    public void LoadScene(string scene_name, ScenesBase sceneBase)
    {
        // 检查场景是否已存在于字典中
        if(!dict_scenes.ContainsKey(scene_name))
        {
            dict_scenes.Add(scene_name, sceneBase);
        }
        
        // 退出当前场景
        string currentSceneName = SceneManager.GetActiveScene().name;
        if(dict_scenes.ContainsKey(currentSceneName))
        {
            // 尝试调用ExitScene方法
            ScenesBase currentScene = dict_scenes[currentSceneName];
            if(currentScene != null)
            {
                currentScene.ExitScene();
            }
        }
        else
        {
            Debug.Log($"当前场景不存在于字典中: {currentSceneName}");
        }

        #region 

        GameRoot.GetInstance().UIManager_Root.Pop(true);
        #endregion
        // 加载新场景
        SceneManager.LoadScene(scene_name);
        sceneBase.EnterScene();
    }
    
  
    
    
}
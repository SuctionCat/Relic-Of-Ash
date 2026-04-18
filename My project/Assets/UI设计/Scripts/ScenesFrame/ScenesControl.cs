using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesControl 
{
    private Dictionary<string, ScenesBase> dict_scenes;
    private static ScenesControl instance;
    
    private ScenesControl()
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
        
        // 加载新场景
        SceneManager.LoadScene(scene_name);
    }
    
    public void AddScene(string scene_name, ScenesBase sceneBase)
    {
        if(!dict_scenes.ContainsKey(scene_name))
        {
            dict_scenes.Add(scene_name, sceneBase);
        }
    }
    
    public ScenesBase GetScene(string scene_name)
    {
        if(dict_scenes.ContainsKey(scene_name))
        {
            return dict_scenes[scene_name];
        }
        return null;
    }
}
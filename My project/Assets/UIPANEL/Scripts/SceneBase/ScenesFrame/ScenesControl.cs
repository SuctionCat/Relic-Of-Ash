using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesControl 
{
    public Dictionary<string, ScenesBase> dict_scenes;
    private static ScenesControl instance;
    
    private LoadingPanel loadingPanel;
    private bool isLoadingScene = false;
    
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
        if(isLoadingScene)
        {
            Debug.LogWarning("正在加载场景中，请等待");
            return;
        }
        
        GameRoot.GetInstance().StartCoroutine(LoadSceneCoroutine(scene_name, sceneBase, false));
    }
    
    /// <summary>
    /// 异步加载场景（显示加载界面）
    /// </summary>
    /// <param name="scene_name">目标场景名称</param>
    /// <param name="sceneBase">场景基类</param>
    public void LoadSceneAsync(string scene_name, ScenesBase sceneBase)
    {
        if(isLoadingScene)
        {
            Debug.LogWarning("正在加载场景中，请等待");
            return;
        }
        
        GameRoot.GetInstance().StartCoroutine(LoadSceneCoroutine(scene_name, sceneBase, true));
    }
    
    private IEnumerator LoadSceneCoroutine(string scene_name, ScenesBase sceneBase, bool showLoadingPanel)
    {
        isLoadingScene = true;
        
        if(!dict_scenes.ContainsKey(scene_name))
        {
            dict_scenes.Add(scene_name, sceneBase);
        }
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        if(dict_scenes.ContainsKey(currentSceneName))
        {
            ScenesBase currentScene = dict_scenes[currentSceneName];
            if(currentScene != null)
            {
                currentScene.ExitScene();
            }
        }
        
        GameRoot.GetInstance().UIManager_Root.Pop(true);
        
        if(showLoadingPanel)
        {
            loadingPanel = new LoadingPanel();
            GameRoot.GetInstance().UIManager_Root.Push(loadingPanel);
            loadingPanel.StartLoading();
            loadingPanel.SetProgress(0f, "正在加载游戏资源...");
        }
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene_name);
        asyncOperation.allowSceneActivation = false;
        
        while(!asyncOperation.isDone)
        {
            float progress = 0f;
            
            if(asyncOperation.progress < 0.9f)
            {
                progress = asyncOperation.progress;
            }
            else
            {
                progress = Mathf.Lerp(0.9f, 1f, (asyncOperation.progress - 0.9f) * 10f);
            }
            
            Debug.Log($"ScenesControl: Scene loading progress: {asyncOperation.progress}, displayed: {progress}");
            
            if(loadingPanel != null && loadingPanel.ActiveObj != null)
            {
                loadingPanel.SetProgress(progress, GetLoadingTip(progress));
            }
            
            if(asyncOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }
            
            yield return null;
        }
        
        if(loadingPanel != null)
        {
            loadingPanel.SetProgress(1f, "加载完成！");
            loadingPanel.StopLoading();
        }
        
        yield return new WaitForSeconds(0.2f);
        
        if(loadingPanel != null)
        {
            GameRoot.GetInstance().UIManager_Root.Pop(true);
            loadingPanel = null;
        }
        
        asyncOperation.allowSceneActivation = true;
        
        while(!asyncOperation.isDone)
        {
            yield return null;
        }
        
        yield return null;
        
        GameObject newCanvas = UImchud.GetInstance().FindCanvas();
        if(newCanvas != null)
        {
            GameRoot.GetInstance().UIManager_Root.CanvasObj = newCanvas;
            Debug.Log($"场景切换后找到Canvas对象");
        }
        else
        {
            Debug.LogWarning("场景切换后未找到Canvas对象");
        }
        
        sceneBase.EnterScene();
        dict_scenes[scene_name] = sceneBase;
        
        isLoadingScene = false;
    }
    
    private string GetLoadingTip(float progress)
    {
        if(progress < 0.3f)
        {
            return "正在加载游戏资源...";
        }
        else if(progress < 0.6f)
        {
            return "正在加载场景数据...";
        }
        else if(progress < 0.9f)
        {
            return "正在初始化场景...";
        }
        else
        {
            return "即将进入游戏...";
        }
    }
    
  
    
    
}
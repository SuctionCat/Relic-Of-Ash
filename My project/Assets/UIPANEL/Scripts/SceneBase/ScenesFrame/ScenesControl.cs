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
        Debug.Log($"ScenesControl.LoadScene: scene_name={scene_name}, isLoadingScene={isLoadingScene}");
        
        if(isLoadingScene)
        {
            Debug.LogWarning("正在加载场景中，强制重置状态");
            // 强制重置加载状态，避免卡住
            isLoadingScene = false;
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
        Debug.Log($"ScenesControl.LoadSceneAsync: scene_name={scene_name}, isLoadingScene={isLoadingScene}");
        
        if(isLoadingScene)
        {
            Debug.LogWarning("正在加载场景中，强制重置状态");
            // 强制重置加载状态，避免卡住
            isLoadingScene = false;
        }
        
        GameRoot.GetInstance().StartCoroutine(LoadSceneCoroutine(scene_name, sceneBase, true));
    }
    
    private IEnumerator LoadSceneCoroutine(string scene_name, ScenesBase sceneBase, bool showLoadingPanel)
    {
        Debug.Log($"LoadSceneCoroutine: ========== 开始加载场景 {scene_name} ==========");
        Debug.Log($"LoadSceneCoroutine: 当前活动场景: {SceneManager.GetActiveScene().name}");
        Debug.Log($"LoadSceneCoroutine: 当前isLoadingScene状态: {isLoadingScene}");
        isLoadingScene = true;
        
        // 清空UI栈，确保新场景有干净的UI状态
        int initialStackSize = GameRoot.GetInstance().UIManager_Root.stack_ui.Count;
        Debug.Log($"LoadSceneCoroutine: 清空UI栈，当前栈大小: {initialStackSize}");
        
        if(initialStackSize > 0)
        {
            Debug.Log("LoadSceneCoroutine: 栈中面板列表:");
            foreach(var panel in GameRoot.GetInstance().UIManager_Root.stack_ui)
            {
                Debug.Log($"  - {panel.uiType.Name}");
            }
        }
        
        while(GameRoot.GetInstance().UIManager_Root.stack_ui.Count > 0)
        {
            try
            {
                GameRoot.GetInstance().UIManager_Root.Pop(true);
                Debug.Log($"LoadSceneCoroutine: 弹出一个面板，剩余栈大小: {GameRoot.GetInstance().UIManager_Root.stack_ui.Count}");
            }
            catch(System.Exception e)
            {
                Debug.LogError($"LoadSceneCoroutine: 弹出面板异常: {e.Message}");
                break;
            }
        }
        Debug.Log($"LoadSceneCoroutine: UI栈清空完成，共弹出 {initialStackSize} 个面板");
        Debug.Log($"LoadSceneCoroutine: 清空后栈大小: {GameRoot.GetInstance().UIManager_Root.stack_ui.Count}");
        Debug.Log($"LoadSceneCoroutine: 清空后字典大小: {GameRoot.GetInstance().UIManager_Root.dict_uiObject.Count}");
        
        if(!dict_scenes.ContainsKey(scene_name))
        {
            dict_scenes.Add(scene_name, sceneBase);
            Debug.Log($"LoadSceneCoroutine: 添加场景到字典 {scene_name}");
        }
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"LoadSceneCoroutine: 当前场景 {currentSceneName}");
        
        if(dict_scenes.ContainsKey(currentSceneName))
        {
            ScenesBase currentScene = dict_scenes[currentSceneName];
            if(currentScene != null)
            {
                Debug.Log($"LoadSceneCoroutine: 调用 {currentSceneName}.ExitScene()");
                currentScene.ExitScene();
                Debug.Log($"LoadSceneCoroutine: {currentSceneName}.ExitScene() 调用完成");
            }
        }
        else
        {
            Debug.LogWarning($"LoadSceneCoroutine: 当前场景 {currentSceneName} 不在字典中");
        }
        
        Debug.Log("LoadSceneCoroutine: 调用 UIManager.Pop(true)");
        try
        {
            GameRoot.GetInstance().UIManager_Root.Pop(true);
            Debug.Log("LoadSceneCoroutine: UIManager.Pop(true) 成功");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"LoadSceneCoroutine: UIManager.Pop(true) 出现异常: {e.Message}");
            // 继续执行场景加载，不被异常打断
        }
        
        if(showLoadingPanel)
        {
            loadingPanel = new LoadingPanel();
            GameRoot.GetInstance().UIManager_Root.Push(loadingPanel);
            Debug.Log("ScenesControl: Loading panel shown");
            
            yield return new WaitForSeconds(1.0f);
            
            Debug.Log("ScenesControl: UI fully rendered");
        }
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene_name);
        asyncOperation.allowSceneActivation = false;
        
        while(!asyncOperation.isDone)
        {
            Debug.Log($"ScenesControl: Scene loading progress: {asyncOperation.progress}");
            
            if(asyncOperation.progress >= 0.9f)
            {
                break;
            }
            
            yield return null;
        }
        
        Debug.Log("ScenesControl: Scene loading completed, waiting for all animations");
        
        while(loadingPanel != null && !loadingPanel.IsAllAnimationsFinished)
        {
            loadingPanel.CheckAllAnimationsFinished();
            yield return null;
        }
        Debug.Log("ScenesControl: All animations finished, proceeding to activate scene");
        
        if(loadingPanel != null)
        {
            Debug.Log("ScenesControl: 开始移除 LoadingPanel");
            try
            {
                GameRoot.GetInstance().UIManager_Root.Pop(true);
                Debug.Log("ScenesControl: LoadingPanel 移除成功");
            }
            catch(System.Exception e)
            {
                Debug.LogError($"ScenesControl: 移除 LoadingPanel 异常: {e.Message}");
            }
            loadingPanel = null;
        }
        
        Debug.Log($"LoadSceneCoroutine: 允许场景激活: {scene_name}");
        asyncOperation.allowSceneActivation = true;
        
        while(!asyncOperation.isDone)
        {
            yield return null;
        }
        
        Debug.Log($"LoadSceneCoroutine: 场景 {scene_name} 激活完成");
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
        
        Debug.Log($"LoadSceneCoroutine: 调用 {scene_name}.EnterScene()");
        sceneBase.EnterScene();
        Debug.Log($"LoadSceneCoroutine: {scene_name}.EnterScene() 调用完成");
        
        dict_scenes[scene_name] = sceneBase;
        
        isLoadingScene = false;
        Debug.Log($"LoadSceneCoroutine: 场景 {scene_name} 加载完成");
        Debug.Log($"LoadSceneCoroutine: 当前场景名称: {SceneManager.GetActiveScene().name}");
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
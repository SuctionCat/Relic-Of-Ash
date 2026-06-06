using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScene : ScenesBase
{
   public readonly string SceneName="MenuScene";
    public override void EnterScene()
    {
        Debug.Log("MenuScene.EnterScene: 开始执行");
        
        // 设置当前场景名称
        GameRoot.GetInstance().SetCurrentSceneName(SceneName);
        Debug.Log($"MenuScene.EnterScene: 场景名称已设置为 {SceneName}");
        
        // 在主菜单场景中，鼠标应该可见且未锁定
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("MenuScene.EnterScene: 鼠标已设置为可见");
        
        // 延迟加载 StartPanel，确保新场景的Canvas已经完全初始化
        Debug.Log("MenuScene.EnterScene: 开始启动协程 LoadStartPanelCoroutine");
        GameRoot.GetInstance().StartCoroutine(LoadStartPanelCoroutine());
        Debug.Log("MenuScene.EnterScene: 协程已启动");
    }
    
    private IEnumerator LoadStartPanelCoroutine()
    {
        yield return null;
        yield return null;
        
        Debug.Log("MenuScene: 开始加载 StartPanel");
        GameRoot.GetInstance().UIManager_Root.Push(new StartPanel());
        Debug.Log("MenuScene: StartPanel 加载完成");
        
        // 检查对象是否存在
        yield return null; // 等待一帧让对象创建完成
        yield return null; // 再等待一帧确保所有操作完成
        
        GameObject startPanelObj = null;
        var uiManager = GameRoot.GetInstance().UIManager_Root;
        
        Debug.Log($"MenuScene: dict_uiObject 包含 {uiManager.dict_uiObject.Count} 个对象");
        foreach(var kvp in uiManager.dict_uiObject)
        {
            Debug.Log($"MenuScene: dict_uiObject 包含: {kvp.Key}");
        }
        
        if(uiManager.dict_uiObject.ContainsKey("StartPanel"))
        {
            startPanelObj = uiManager.dict_uiObject["StartPanel"];
        }
        
        if(startPanelObj != null)
        {
            Debug.Log($"MenuScene: StartPanel对象存在: {startPanelObj.name}");
            Debug.Log($"MenuScene: active={startPanelObj.activeSelf}");
            Debug.Log($"MenuScene: scene={startPanelObj.scene.name}, 当前场景={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            Debug.Log($"MenuScene: 父对象={startPanelObj.transform.parent?.name}");
            Debug.Log($"MenuScene: 父对象场景={startPanelObj.transform.parent?.gameObject.scene.name}");
            
            // 检查Canvas组件
            Canvas canvas = startPanelObj.GetComponentInParent<Canvas>();
            if(canvas != null)
            {
                Debug.Log($"MenuScene: Canvas组件存在: {canvas.name}, enabled={canvas.enabled}, renderMode={canvas.renderMode}");
            }
            else
            {
                Debug.LogError("MenuScene: 未找到Canvas组件");
            }
            
            // 检查RectTransform
            RectTransform rt = startPanelObj.GetComponent<RectTransform>();
            if(rt != null)
            {
                Debug.Log($"MenuScene: RectTransform存在: position={rt.position}, sizeDelta={rt.sizeDelta}");
            }
            else
            {
                Debug.LogError("MenuScene: 未找到RectTransform组件");
            }
        }
        else
        {
            Debug.LogError("MenuScene: StartPanel对象不存在于字典中");
        }
    }
    
    public override void ExitScene()
    {
        // 清除当前场景名称
        GameRoot.GetInstance().SetCurrentSceneName("");
    }
}

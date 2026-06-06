using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImchud 
{
    private static UImchud instance;
    public static UImchud GetInstance()
    {
        if(instance==null)
        {
           Debug.Log("UImchud实例不存在");
           instance = new UImchud();
           Debug.Log("UImchud实例已创建");
        }
        
        return instance;
    }
    public GameObject FindCanvas()
    {
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // 优先查找当前场景中的Canvas
        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.scene.name == currentSceneName)
            {
                Debug.Log($"找到当前场景 {currentSceneName} 的Canvas: {canvas.name}");
                return canvas.gameObject;
            }
        }
        
        // 如果没找到当前场景的Canvas，返回第一个找到的（保持兼容性）
        if (canvases.Length > 0)
        {
            Debug.LogWarning($"未找到当前场景 {currentSceneName} 的Canvas，使用第一个Canvas: {canvases[0].name}");
            return canvases[0].gameObject;
        }
        
        Debug.Log("未能成功获得Canvas物体");
        return null;
    }
    public T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if(component == null)
        {
            component = obj.AddComponent<T>();
        }
        return component;
    }
    public T GetOrAddComponent<T>(GameObject parent, string childName) where T : Component
    {
        GameObject child = FindObjectInChill(parent, childName);
        if(child == null)
        {
            child = GetOrAddGameObject(parent, childName);
        }
        return GetOrAddComponent<T>(child);
    }
    public GameObject FindObjectInChill(GameObject panel, string ChillName)
   {
       Transform[] transforms=panel.GetComponentsInChildren<Transform>();
   
       foreach(var transform in transforms)
       {
           if(transform.gameObject.name == ChillName)
           {
               return transform.gameObject;
           }
       }
       return null;
   }
   public GameObject GetOrAddGameObject(GameObject panel, string ChillName)
   {
       GameObject gameObject = FindObjectInChill(panel, ChillName);
       if(gameObject == null)
       {
           gameObject = new GameObject(ChillName);
           gameObject.transform.SetParent(panel.transform);
           return gameObject;
       }
       return gameObject;
   }//需要修改
}

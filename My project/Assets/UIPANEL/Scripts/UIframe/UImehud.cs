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
        GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        if(canvas == null)
        {
            Debug.Log($"未能成功获得Canvas物体");
            return null;
        }
        return canvas;
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
       Debug.Log($"未能成功获得{ChillName}物体");
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

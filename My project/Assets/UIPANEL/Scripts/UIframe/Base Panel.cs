using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BasePanel 
{
    public UITepy uiType;
    //<summary>
    ///此Panel在场景里对应的物体
    /// </summary>
    public GameObject ActiveObj;
    public BasePanel(UITepy uitype)
    {
        uiType = uitype;
    }
    public virtual void ONStart()
    {
        UImchud.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj).interactable = true;
    }
    public virtual void OnEnable()
    {
        UImchud.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj).interactable = true;
    }
    public virtual void OnDisable()
    {
        UImchud.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj).interactable = false;
    }
    public virtual void OnDestroy()
    {
        UImchud.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj).interactable = false;

    }
}

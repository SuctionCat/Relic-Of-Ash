using System.Collections;
using System.Collections.Generic;
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
        
    }
    public virtual void OnEnable()
    {
        
    }
    public virtual void OnDisable()
    {
        
    }
    public virtual void OnDestroy()
    {
        
    }
}

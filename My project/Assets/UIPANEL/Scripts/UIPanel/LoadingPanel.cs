using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : BasePanel
{
    private static string Name = "LoadingPanel";
    private static string Path = "Panel/LoadingPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path, Name);
    
    private List<Animator> allAnimators = new List<Animator>();
    private bool isAllAnimationsFinished = false;
    public bool IsAllAnimationsFinished { get => isAllAnimationsFinished; }
    
    public LoadingPanel() : base(UIPanelType) { }
    
    public override void ONStart()
    {
        base.ONStart();
        
        Animator[] animators = ActiveObj.GetComponentsInChildren<Animator>(true);
        if(animators != null && animators.Length > 0)
        {
            allAnimators.AddRange(animators);
            Debug.Log($"LoadingPanel: Found {allAnimators.Count} animator(s)");
        }
        else
        {
            Debug.LogWarning("LoadingPanel: No animators found");
        }
        
        isAllAnimationsFinished = false;
        Debug.Log("LoadingPanel: Initialized");
    }
    
    public void CheckAllAnimationsFinished()
    {
        if(allAnimators.Count == 0)
        {
            isAllAnimationsFinished = true;
            Debug.Log("LoadingPanel: No animators, skipping animation wait");
            return;
        }
        
        foreach(Animator animator in allAnimators)
        {
            if(animator != null && animator.enabled && animator.gameObject.activeInHierarchy)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if(stateInfo.loop || stateInfo.normalizedTime < 1f)
                {
                    return;
                }
            }
        }
        
        isAllAnimationsFinished = true;
        Debug.Log("LoadingPanel: All animations finished");
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}

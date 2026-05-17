using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingPanel : BasePanel
{
    private static string Name = "LoadingPanel";
    private static string Path = "Panel/LoadingPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path, Name);
    
    private Slider progressSlider;
    private Image fillImage;
    private TextMeshProUGUI progressText;
    private TextMeshProUGUI tipText;
    
    private float targetProgress = 0f;
    private float currentProgress = 0f;
    private float smoothSpeed = 10f;
    
    private bool isLoading = false;
    
    public LoadingPanel() : base(UIPanelType) { }
    
    public override void ONStart()
    {
        base.ONStart();
        
        progressSlider = UImchud.GetInstance().GetOrAddComponent<Slider>(ActiveObj, "ProgressSlider");
        tipText = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj, "TipText");
        
        Transform fillRect = ActiveObj.transform.Find("ProgressSlider/Fill Area/Fill");
        if(fillRect != null)
        {
            fillImage = fillRect.GetComponent<Image>();
        }
        
        Transform progressTextTransform = ActiveObj.transform.Find("ProgressSlider/ProgressText");
        if(progressTextTransform != null)
        {
            progressText = progressTextTransform.GetComponent<TextMeshProUGUI>();
        }
        
        if(progressSlider != null)
        {
            progressSlider.value = 0f;
            Debug.Log("LoadingPanel: ProgressSlider found and initialized");
        }
        else
        {
            Debug.LogError("LoadingPanel: ProgressSlider not found!");
        }
        
        if(tipText != null)
        {
            Debug.Log("LoadingPanel: TipText found");
        }
        else
        {
            Debug.LogError("LoadingPanel: TipText not found!");
        }
        
        GameRoot.GetInstance().RegisterUpdateMethod(UpdateProgress);
        Debug.Log("LoadingPanel: UpdateProgress registered");
    }
    
    private void UpdateProgress()
    {
        if(!isLoading) return;
        if(ActiveObj == null) return;
        
        currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, Time.deltaTime * smoothSpeed);
        
        if(progressSlider != null)
        {
            progressSlider.value = currentProgress;
            Debug.Log($"LoadingPanel: Progress updated to {Mathf.RoundToInt(currentProgress * 100)}% (target: {Mathf.RoundToInt(targetProgress * 100)}%)");
        }
        
        if(progressText != null)
        {
            progressText.text = Mathf.RoundToInt(currentProgress * 100) + "%";
        }
    }
    
    public void SetProgress(float progress, string tip = "")
    {
        targetProgress = Mathf.Clamp01(progress);
        
        if(tipText != null && !string.IsNullOrEmpty(tip))
        {
            tipText.text = tip;
        }
    }
    
    public void StartLoading()
    {
        isLoading = true;
        currentProgress = 0f;
        targetProgress = 0f;
    }
    
    public void StopLoading()
    {
        isLoading = false;
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        GameRoot.GetInstance().UnregisterUpdateMethod(UpdateProgress);
    }
}
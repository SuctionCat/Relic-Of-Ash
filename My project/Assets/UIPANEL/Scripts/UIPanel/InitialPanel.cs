using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitialPanel : BasePanel
{
    private static string Name = "InitialPanel";
    private static string Path = "Panel/InitialPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path, Name);

    private Text textComponent;
    private TMP_Text tmpTextComponent;
    private Image backgroundImage;
    private float glowSpeed = 1.5f;
    private float minAlpha = 0.3f;
    private float maxAlpha = 1.0f;
    private bool hasPressedAnyKey = false;
    private float backgroundPulseSpeed = 1.0f;
    private float backgroundScaleMin = 1f;
    private float backgroundScaleMax = 1.1f;

    public InitialPanel() : base(UIPanelType)
    {
    }

    public override void ONStart()
    {
        base.ONStart();
        Debug.Log("InitialPanel ONStart called");
        // 尝试获取名为PromptText的Text组件
        textComponent = UImched.GetInstance().GetOrAddComponent<Text>(ActiveObj, "PromptText");
        if (textComponent != null)
        {
            Debug.Log("成功获取到PromptText Text组件");
        }
        else
        {
            // 尝试获取TMP_Text组件
            tmpTextComponent = UImched.GetInstance().GetOrAddComponent<TMP_Text>(ActiveObj, "PromptText");
            if (tmpTextComponent != null)
            {
                Debug.Log("成功获取到PromptText TMP_Text组件");
            }
            else
            {
                // 如果没有找到PromptText，尝试获取第一个Text或TMP_Text组件
                Text[] textComponents = ActiveObj.GetComponentsInChildren<Text>();
                TMP_Text[] tmpTextComponents = ActiveObj.GetComponentsInChildren<TMP_Text>();
                
                if (textComponents.Length > 0)
                {
                    textComponent = textComponents[0];
                    Debug.Log("未找到PromptText组件，使用第一个Text组件");
                }
                else if (tmpTextComponents.Length > 0)
                {
                    tmpTextComponent = tmpTextComponents[0];
                    Debug.Log("未找到PromptText组件，使用第一个TMP_Text组件");
                }
                else
                {
                    // 如果没有Text组件，创建一个
                    GameObject textObj = new GameObject("PromptText");
                    textObj.transform.SetParent(ActiveObj.transform);
                    textObj.transform.localPosition = Vector3.zero;
                    textComponent = textObj.AddComponent<Text>();
                    // 设置Text组件的基本属性
                    textComponent.text = "按任意键进入游戏";
                    textComponent.fontSize = 36;
                    textComponent.alignment = TextAnchor.MiddleCenter;
                    textComponent.color = Color.white;
                    Debug.Log("未找到Text组件，创建了一个新的Text组件");
                }
            }
        }

        // 获取BackGround图片
        backgroundImage = UImched.GetInstance().GetOrAddComponent<Image>(ActiveObj, "BackGround");
        if (backgroundImage != null)
        {
            Debug.Log("成功获取到BackGround图片组件");
        }
        else
        {
            Debug.Log("未找到BackGround图片组件，背景律动效果将不会生效");
        }

        // 注册Update方法
        GameRoot.GetInstance().RegisterUpdateMethod(UpdateMethod);
        Debug.Log("UpdateMethod已注册");
    }

    private void UpdateMethod()
    {
        UpdateGlowEffect();
        UpdateBackGroundEffect();
        CheckForAnyKey();
    }

    private void UpdateGlowEffect()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * glowSpeed) + 1) / 2);
        
        if (textComponent != null)
        {
            Color color = textComponent.color;
            color.a = alpha;
            textComponent.color = color;
        }
        else if (tmpTextComponent != null)
        {
            Color color = tmpTextComponent.color;
            color.a = alpha;
            tmpTextComponent.color = color;
        }
        
        // 每5帧输出一次调试信息
        if (Time.frameCount % 5 == 0)
        {
            Debug.Log($"文字透明度: {alpha}");
        }
    }

    private void UpdateBackGroundEffect()
    {
        if (backgroundImage == null) return;

        // 计算缩放因子
        float scale = Mathf.Lerp(backgroundScaleMin, backgroundScaleMax, (Mathf.Sin(Time.time * backgroundPulseSpeed) + 1) / 2);
        
        // 应用缩放
        backgroundImage.transform.localScale = new Vector3(scale, scale, 1f);
        
        // 计算透明度变化
        float alpha = Mathf.Lerp(0.7f, 1.0f, (Mathf.Sin(Time.time * backgroundPulseSpeed * 0.8f) + 1) / 2);
        
        // 应用透明度
        Color color = backgroundImage.color;
        color.a = alpha;
        backgroundImage.color = color;
    }

    private void CheckForAnyKey()
    {
        if (hasPressedAnyKey) return;

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            hasPressedAnyKey = true;
            AudioManager.PlayClick();
            TransitionToNextPanel();
        }
    }

    private void TransitionToNextPanel()
    {
        // 先Push StartPanel，让它成为新的栈顶面板
        GameRoot.GetInstance().UIManager_Root.Push(new StartPanel());
        
        // 现在InitialPanel应该在栈的倒数第二个位置
        // 我们需要获取UIManager的栈，然后移除InitialPanel
        // 但是UIManager的stack_ui是私有的，我们需要另一种方法
        
        // 方法：先Pop所有面板，然后再Push StartPanel
        // 这样InitialPanel就会被完全销毁
        GameRoot.GetInstance().UIManager_Root.Pop(true); // 销毁所有面板
        GameRoot.GetInstance().UIManager_Root.Push(new StartPanel()); // 重新Push StartPanel
    }

    public override void OnEnable()
    {
        base.OnEnable();
        hasPressedAnyKey = false;
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnDestroy()
    {
        // 取消注册Update方法
        GameRoot.GetInstance().UnregisterUpdateMethod(UpdateMethod);
        base.OnDestroy();
    }
}
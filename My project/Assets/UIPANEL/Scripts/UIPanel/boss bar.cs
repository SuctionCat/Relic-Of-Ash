using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBarPanel : BasePanel
{
    private static string Name = "BossBarPanel";
    private static string Path = "Panel/BossBarPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path, Name);

    // UI组件引用
    private Image healthBarImage;

    // 动画相关
    private float targetHealthPercent = 1f;
    private float smoothSpeed = 5f;

    // Boss数据
    private float maxHealth = 0f;
    private float currentHealth = 0f;

    public BossBarPanel() : base(UIPanelType)
    {
    }

    public override void ONStart()
    {
        base.ONStart();

        // 获取Bar Image对象下的Image组件（使用图片填充显示血量）
        GameObject barImageObject = UImchud.GetInstance().FindObjectInChill(ActiveObj, "Bar Image");
        if (barImageObject != null)
        {
            healthBarImage = barImageObject.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("BossBarPanel: 未获取到 Bar Image 对象，请检查UI层级结构");
        }

        // 注册Update方法到游戏循环
        GameRoot.GetInstance().RegisterUpdateMethod(Update);

        // 初始隐藏面板
        SetVisible(false);
    }

    // 设置面板可见性
    public void SetVisible(bool visible)
    {
        if (ActiveObj != null)
        {
            ActiveObj.SetActive(visible);
        }
    }

    // 设置Boss数据
    public void SetBossData(float maxHealth, float currentHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        targetHealthPercent = currentHealth / maxHealth;

        // 延迟获取Image组件（解决初始化顺序问题）
        EnsureHealthBarImage();

        SetVisible(true);
    }

    // 确保获取到血条Image组件
    private void EnsureHealthBarImage()
    {
        if (healthBarImage == null && ActiveObj != null)
        {
            GameObject barImageObject = UImchud.GetInstance().FindObjectInChill(ActiveObj, "Bar/Bar Image");
            if (barImageObject != null)
            {
                healthBarImage = barImageObject.GetComponent<Image>();
                if (healthBarImage != null)
                {
                    Debug.Log("BossBarPanel: 延迟获取 Bar/Bar Image 成功");
                }
            }
        }
    }

    // 更新生命值
    public void UpdateHealth(float newHealth)
    {
        currentHealth = newHealth;
        targetHealthPercent = currentHealth / maxHealth;

        // 延迟获取Image组件（解决初始化顺序问题）
        EnsureHealthBarImage();
    }

    // 更新UI显示（仅处理平滑动画）
    private void UpdateHealthUI()
    {
        if (healthBarImage == null || maxHealth <= 0) return;

        healthBarImage.fillAmount = Mathf.MoveTowards(healthBarImage.fillAmount, targetHealthPercent, Time.deltaTime * smoothSpeed);
    }

    private void Update()
    {
        // 更新血条动画
        UpdateHealthUI();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnDestroy()
    {
        GameRoot.GetInstance().UnregisterUpdateMethod(Update);
        base.OnDestroy();
    }
}
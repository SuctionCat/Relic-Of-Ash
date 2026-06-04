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

        // 获取Bar对象下的Image组件（使用图片填充显示血量）
        GameObject barObject = UImchud.GetInstance().FindObjectInChill(ActiveObj, "Bar");
        if (barObject != null)
        {
            healthBarImage = barObject.GetComponent<Image>();
            if (healthBarImage == null)
            {
                healthBarImage = barObject.AddComponent<Image>();
            }
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

        UpdateHealthUI();
        SetVisible(true);
    }

    // 更新生命值
    public void UpdateHealth(float newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthUI();
    }

    // 更新UI显示
    private void UpdateHealthUI()
    {
        if (maxHealth <= 0) return;

        targetHealthPercent = currentHealth / maxHealth;

        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = Mathf.MoveTowards(healthBarImage.fillAmount, targetHealthPercent, Time.deltaTime * smoothSpeed);
        }
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
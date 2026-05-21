using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsPanel : BasePanel
{
    private static string Name="PlayerStatsPanel";
    private static string Path="Panel/PlayerStatsPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);

    // UI组件引用
    private Image healthBarImage;
    private Image shieldBarImage;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI shieldText;
    private TextMeshProUGUI healthText1;
    private TextMeshProUGUI shieldText1;

    // 动画相关
    private float targetHealthPercent = 1f;
    private float targetShieldPercent = 1f;
    private float smoothSpeed = 5f;
    
    // 技能按钮引用
    private Button buttonE;
    private Button buttonQ;
    
    // 冷却时间文本（可选）
    private TextMeshProUGUI cooldownTextE;
    private TextMeshProUGUI cooldownTextQ;

public PlayerStatsPanel():base(UIPanelType)
{
    // uiType is already set in the base constructor
}
    // Start is called before the first frame update
    public override void ONStart()
    {
        base.ONStart();
        UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"Back").onClick.AddListener(BackButtonClick);
        
        // 获取血量UI组件
        healthBarImage = UImchud.GetInstance().GetOrAddComponent<Image>(ActiveObj, "HealthBar");
        shieldBarImage = UImchud.GetInstance().GetOrAddComponent<Image>(ActiveObj, "ShieldBar");
        healthText = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj, "HealthText");
        shieldText = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj, "ShieldText");
        healthText1 = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj, "HealthText1");
        shieldText1 = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj, "ShieldText1");
        
        // 获取技能按钮引用
        buttonE = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"ButtonE");
        buttonQ = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"ButtonQ");
        
        // 尝试获取冷却时间文本（如果存在）
        cooldownTextE = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj,"CooldownTextE");
        cooldownTextQ = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj,"CooldownTextQ");
        
        // 初始隐藏冷却文本
        if(cooldownTextE != null)
        {
            cooldownTextE.gameObject.SetActive(false);
        }
        if(cooldownTextQ != null)
        {
            cooldownTextQ.gameObject.SetActive(false);
        }
        
        // 注册Update方法到游戏循环
        GameRoot.GetInstance().RegisterUpdateMethod(Update);
    }
    
    // 更新血量UI
    private void UpdateHealthUI()
    {
        if(StateManager.instance == null) return;
        
        float maxHealth = PlayerMemento.instance != null ? PlayerMemento.instance.GetInitialMaxHealth() : 1000f;
        float maxShield = PlayerMemento.instance != null ? PlayerMemento.instance.GetInitialMaxShield() : 200f;
        
        float currentHealth = StateManager.instance.GetHealth();
        float currentShield = StateManager.instance.GetShield();
        
        targetHealthPercent = currentHealth / maxHealth;
        targetShieldPercent = currentShield / maxShield;
        
        if(healthBarImage != null)
        {
            healthBarImage.fillAmount = Mathf.MoveTowards(healthBarImage.fillAmount, targetHealthPercent, Time.deltaTime * smoothSpeed);
        }
        
        if(shieldBarImage != null)
        {
            shieldBarImage.fillAmount = Mathf.MoveTowards(shieldBarImage.fillAmount, targetShieldPercent, Time.deltaTime * smoothSpeed);
        }
        
        if(healthText != null)
        {
            healthText.text = Mathf.Ceil(currentHealth).ToString();
        }
        if(healthText1 != null)
        {
            healthText1.text = Mathf.Ceil(currentHealth).ToString();
        }
        
        if(shieldText != null)
        {
            shieldText.text = Mathf.Ceil(currentShield).ToString();
        }
        if(shieldText1 != null)
        {
            shieldText1.text = Mathf.Ceil(currentShield).ToString();
        }
    }
    
    private void Update()
    {
        // 更新技能冷却显示
        UpdateCooldownDisplay();
        
        // 更新血量UI
        UpdateHealthUI();
    }
    
    private void UpdateCooldownDisplay()
    {
        if(StateManager.instance == null) return;
        
        // 更新技能E冷却
        float eCooldown = StateManager.instance.GetECooldownRemaining();
        if(eCooldown > 0)
        {
            buttonE.interactable = false;
            if(cooldownTextE != null)
            {
                cooldownTextE.text = Mathf.Ceil(eCooldown).ToString();
                cooldownTextE.gameObject.SetActive(true);
            }
        }
        else
        {
            buttonE.interactable = true;
            if(cooldownTextE != null)
            {
                cooldownTextE.gameObject.SetActive(false);
            }
        }
        
        // 更新技能Q冷却
        float qCooldown = StateManager.instance.GetQCooldownRemaining();
        if(qCooldown > 0)
        {
            buttonQ.interactable = false;
            if(cooldownTextQ != null)
            {
                cooldownTextQ.text = Mathf.Ceil(qCooldown).ToString();
                cooldownTextQ.gameObject.SetActive(true);
            }
        }
        else
        {
            buttonQ.interactable = true;
            if(cooldownTextQ != null)
            {
                cooldownTextQ.gameObject.SetActive(false);
            }
        }
    }
    
    private void BackButtonClick()
    {
        AudioManager.PlayClick();
        GameRoot.GetInstance().UIManager_Root.Pop(false);
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        UIManager.GetInstance().DisableAllPanelsExcept(this);
    }
    
    public override void OnDisable()
    {
        Debug.Log("PlayerStatsPanel IS back");
        base.OnDisable();
        UIManager.GetInstance().RestorePreviousPanelInteractivity();
    }
    
    public override void OnDestroy()
    {
        GameRoot.GetInstance().UnregisterUpdateMethod(Update);
        base.OnDestroy();
    }
}
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
    
    // 武器切换相关
    private RectTransform weapon1Rect;
    private RectTransform weapon2Rect;
    private RectTransform weapon3Rect;
    
    // 武器原始位置字典
    private Dictionary<string, Vector3> weaponOriginalPositions = new Dictionary<string, Vector3>();

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
        
        // 获取武器 UI 组件
        weapon1Rect = UImchud.GetInstance().GetOrAddComponent<RectTransform>(ActiveObj, "weapon 1");
        weapon2Rect = UImchud.GetInstance().GetOrAddComponent<RectTransform>(ActiveObj, "weapon 2");
        weapon3Rect = UImchud.GetInstance().GetOrAddComponent<RectTransform>(ActiveObj, "weapon 3");
        
        // 保存原始位置到字典
        if(weapon1Rect != null) weaponOriginalPositions["weapon 1"] = weapon1Rect.localPosition;
        if(weapon2Rect != null) weaponOriginalPositions["weapon 2"] = weapon2Rect.localPosition;
        if(weapon3Rect != null) weaponOriginalPositions["weapon 3"] = weapon3Rect.localPosition;
        
        // 初始化武器显示
        UpdateWeaponDisplay();
        
        // 注册 Update 方法到游戏循环
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
    
    // 更新武器显示（三角形旋转切换）
    private void UpdateWeaponDisplay()
    {
        if(weapon1Rect == null || weapon2Rect == null || weapon3Rect == null)
            return;
        
        // 获取StateManager中的当前武器索引
        int currentWeaponIndex = StateManager.instance != null ? StateManager.instance.GetWeaponIndex() : 0;
        
        // 从字典获取三个武器的原始位置作为三角形的三个顶点
        Vector3[] trianglePoints = new Vector3[3];
        trianglePoints[0] = weaponOriginalPositions["weapon 1"];
        trianglePoints[1] = weaponOriginalPositions["weapon 2"];
        trianglePoints[2] = weaponOriginalPositions["weapon 3"];
        
        // 找出哪个位置Y坐标最大（最靠近屏幕顶部）
        int topPositionIndex = 0;
        float maxY = trianglePoints[0].y;
        for(int i = 1; i < 3; i++)
        {
            if(trianglePoints[i].y > maxY)
            {
                maxY = trianglePoints[i].y;
                topPositionIndex = i;
            }
        }
        
        // 旋转逻辑：根据当前选中的武器索引分配位置
        int[] targetIndices = new int[3];
        for(int i = 0; i < 3; i++)
        {
            targetIndices[i] = (i + currentWeaponIndex) % 3;
        }
        
        // 先更新所有武器的位置和缩放
        bool weapon1IsTop = false;
        bool weapon2IsTop = false;
        bool weapon3IsTop = false;
        
        // 更新武器 1：放在目标位置，只有在顶部位置时放大
        if(weapon1Rect != null)
        {
            int targetPos = targetIndices[0];
            weapon1Rect.localPosition = trianglePoints[targetPos];
            weapon1Rect.localScale = (targetPos == topPositionIndex) ? Vector3.one * 1.2f : Vector3.one;
            weapon1IsTop = (targetPos == topPositionIndex);
        }
        
        // 更新武器 2：放在目标位置，只有在顶部位置时放大
        if(weapon2Rect != null)
        {
            int targetPos = targetIndices[1];
            weapon2Rect.localPosition = trianglePoints[targetPos];
            weapon2Rect.localScale = (targetPos == topPositionIndex) ? Vector3.one * 1.2f : Vector3.one;
            weapon2IsTop = (targetPos == topPositionIndex);
        }
        
        // 更新武器 3：放在目标位置，只有在顶部位置时放大
        if(weapon3Rect != null)
        {
            int targetPos = targetIndices[2];
            weapon3Rect.localPosition = trianglePoints[targetPos];
            weapon3Rect.localScale = (targetPos == topPositionIndex) ? Vector3.one * 1.2f : Vector3.one;
            weapon3IsTop = (targetPos == topPositionIndex);
        }
        
        // 确保顶部武器显示在最上面
        if(weapon1IsTop && weapon1Rect != null)
            weapon1Rect.SetAsLastSibling();
        else if(weapon2IsTop && weapon2Rect != null)
            weapon2Rect.SetAsLastSibling();
        else if(weapon3IsTop && weapon3Rect != null)
            weapon3Rect.SetAsLastSibling();
    }
    
    private void Update()
    {
        // 更新血量UI
        UpdateHealthUI();
        
        // 更新冷却显示
        UpdateCooldownDisplay();
        
        // 更新武器显示
        UpdateWeaponDisplay();
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

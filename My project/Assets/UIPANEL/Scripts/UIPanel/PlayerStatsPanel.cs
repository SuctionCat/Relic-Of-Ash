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
    
    // 冷却遮罩图片
    private Image cdImageE;
    private Image cdImageQ;
    
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
        try
        {
            Debug.Log($"PlayerStatsPanel.ONStart: ========== 开始初始化 ==========");
            base.ONStart();
            
            Debug.Log($"PlayerStatsPanel.ONStart: ActiveObj={ActiveObj?.name ?? "null"}");
            
            // 获取血量UI组件
            Debug.Log($"PlayerStatsPanel.ONStart: 获取血量UI组件");
            healthBarImage = UImchud.GetInstance().GetOrAddComponent<Image>(ActiveObj, "HealthBar");
            shieldBarImage = UImchud.GetInstance().GetOrAddComponent<Image>(ActiveObj, "ShieldBar");
            healthText = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj, "HealthText");
            shieldText = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj, "ShieldText");
            healthText1 = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj, "HealthText1");
            shieldText1 = UImchud.GetInstance().GetOrAddComponent<TextMeshProUGUI>(ActiveObj, "ShieldText1");
            
            Debug.Log($"PlayerStatsPanel.ONStart: healthBarImage={healthBarImage != null}, shieldBarImage={shieldBarImage != null}");
            Debug.Log($"PlayerStatsPanel.ONStart: healthText={healthText != null}, shieldText={shieldText != null}");
            
            // 获取技能按钮引用
            Debug.Log($"PlayerStatsPanel.ONStart: 获取技能按钮");
            buttonE = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"ButtonE");
            buttonQ = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"ButtonQ");
            
            Debug.Log($"PlayerStatsPanel.ONStart: buttonE={buttonE != null}, buttonQ={buttonQ != null}");
            
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
            
            // 获取冷却遮罩图片
            cdImageE = UImchud.GetInstance().GetOrAddComponent<Image>(ActiveObj,"CD_Image_E");
            cdImageQ = UImchud.GetInstance().GetOrAddComponent<Image>(ActiveObj,"CD_Image_Q");
            
            // 初始化冷却遮罩填充量为0
            if(cdImageE != null)
            {
                cdImageE.fillAmount = 0f;
            }
            if(cdImageQ != null)
            {
                cdImageQ.fillAmount = 0f;
            }
            
            // 获取武器 UI 组件
            Debug.Log($"PlayerStatsPanel.ONStart: 获取武器UI组件");
            weapon1Rect = UImchud.GetInstance().GetOrAddComponent<RectTransform>(ActiveObj, "weapon 1");
            weapon2Rect = UImchud.GetInstance().GetOrAddComponent<RectTransform>(ActiveObj, "weapon 2");
            weapon3Rect = UImchud.GetInstance().GetOrAddComponent<RectTransform>(ActiveObj, "weapon 3");
            
            Debug.Log($"PlayerStatsPanel.ONStart: weapon1Rect={weapon1Rect != null}, weapon2Rect={weapon2Rect != null}, weapon3Rect={weapon3Rect != null}");
            
            // 保存原始位置到字典
            if(weapon1Rect != null) weaponOriginalPositions["weapon 1"] = weapon1Rect.localPosition;
            if(weapon2Rect != null) weaponOriginalPositions["weapon 2"] = weapon2Rect.localPosition;
            if(weapon3Rect != null) weaponOriginalPositions["weapon 3"] = weapon3Rect.localPosition;
            
            // 初始化武器显示
            Debug.Log($"PlayerStatsPanel.ONStart: 初始化武器显示");
            UpdateWeaponDisplay();
            
            // 注册 Update 方法到游戏循环
            Debug.Log($"PlayerStatsPanel.ONStart: 注册Update方法");
            GameRoot.GetInstance().RegisterUpdateMethod(Update);
            
            Debug.Log($"PlayerStatsPanel.ONStart: ========== 初始化完成 ==========");
        }
        catch(System.Exception e)
        {
            Debug.LogError($"PlayerStatsPanel.ONStart: 初始化异常: {e.Message}");
            Debug.LogError($"PlayerStatsPanel.ONStart: 异常堆栈: {e.StackTrace}");
        }
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
        float eMaxCooldown = StateManager.instance.eCooldownTime;
        if(eCooldown > 0)
        {
            buttonE.interactable = false;
            if(cooldownTextE != null)
            {
                cooldownTextE.text = Mathf.Ceil(eCooldown).ToString();
                cooldownTextE.gameObject.SetActive(true);
            }
            // 设置冷却遮罩填充量
            if(cdImageE != null && eMaxCooldown > 0)
            {
                cdImageE.fillAmount = eCooldown / eMaxCooldown;
            }
        }
        else
        {
            buttonE.interactable = true;
            if(cooldownTextE != null)
            {
                cooldownTextE.gameObject.SetActive(false);
            }
            // 重置冷却遮罩填充量为0
            if(cdImageE != null)
            {
                cdImageE.fillAmount = 0f;
            }
        }
        
        // 更新技能Q冷却
        float qCooldown = StateManager.instance.GetQCooldownRemaining();
        float qMaxCooldown = StateManager.instance.qCooldownTime;
        if(qCooldown > 0)
        {
            buttonQ.interactable = false;
            if(cooldownTextQ != null)
            {
                cooldownTextQ.text = Mathf.Ceil(qCooldown).ToString();
                cooldownTextQ.gameObject.SetActive(true);
            }
            // 设置冷却遮罩填充量
            if(cdImageQ != null && qMaxCooldown > 0)
            {
                cdImageQ.fillAmount = qCooldown / qMaxCooldown;
            }
        }
        else
        {
            buttonQ.interactable = true;
            if(cooldownTextQ != null)
            {
                cooldownTextQ.gameObject.SetActive(false);
            }
            // 重置冷却遮罩填充量为0
            if(cdImageQ != null)
            {
                cdImageQ.fillAmount = 0f;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsPanel : BasePanel
{
    private static string Name="PlayerStatsPanel";
    private static string Path="Panel/PlayerStatsPanel";
    public static readonly UITepy UIPanelType = new UITepy(Path,Name);

    // 血量相关
    [Header("血量设置")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    
    // 护盾相关
    [Header("护盾设置")]
    public float maxShield = 100f;
    public float currentShield = 100f;
    
    // 血量回复
    [Header("回复设置")]
    public float healthRegenRate = 1f;
    public float shieldRegenRate = 2f;
    public float regenDelay = 3f;
    private float lastDamageTime = 0f;
    
    // UI组件引用
    private Image healthBarImage;
    private Image shieldBarImage;
    private Text healthText;
    private Text shieldText;
    
    // 动画相关
    private float targetHealthPercent = 1f;
    private float targetShieldPercent = 1f;
    private float smoothSpeed = 5f;
    
    // 技能按钮引用
    private Button buttonE;
    private Button buttonQ;
    
    // 技能冷却时间（秒）
    private float cooldownE = 5f;
    private float cooldownQ = 3f;
    
    // 当前冷却时间
    private float currentCooldownE = 0f;
    private float currentCooldownQ = 0f;
    
    // 冷却时间文本（可选）
    private Text cooldownTextE;
    private Text cooldownTextQ;
    
    // 武器切换相关
    private RectTransform weapon1Rect;
    private RectTransform weapon2Rect;
    private RectTransform weapon3Rect;
    
    // 武器原始位置字典
    private Dictionary<string, Vector3> weaponOriginalPositions = new Dictionary<string, Vector3>();
    
    // 当前激活的武器索引（0=武器 1，1=武器 2，2=武器 3）
    private int currentWeaponIndex = 0;
    
    // 武器切换冷却
    private float weaponSwitchCooldown = 0.5f;
    private float currentWeaponCooldown = 0f;

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
        healthText = UImchud.GetInstance().GetOrAddComponent<Text>(ActiveObj, "HealthText");
        shieldText = UImchud.GetInstance().GetOrAddComponent<Text>(ActiveObj, "ShieldText");
        
        // 初始化血量和护盾
        currentHealth = maxHealth;
        currentShield = maxShield;
        targetHealthPercent = 1f;
        targetShieldPercent = 1f;
        
        // 更新UI显示
        UpdateHealthUI();
        
        // 获取技能按钮引用
        buttonE = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"ButtonE");
        buttonQ = UImchud.GetInstance().GetOrAddComponent<Button>(ActiveObj,"ButtonQ");
        
        // 为技能按钮添加点击事件
        if(buttonE != null)
        {
            buttonE.onClick.AddListener(OnButtonEClick);
        }
        
        if(buttonQ != null)
        {
            buttonQ.onClick.AddListener(OnButtonQClick);
        }
        
        // 尝试获取冷却时间文本（如果存在）
        cooldownTextE = UImchud.GetInstance().GetOrAddComponent<Text>(ActiveObj,"CooldownTextE");
        cooldownTextQ = UImchud.GetInstance().GetOrAddComponent<Text>(ActiveObj,"CooldownTextQ");
        
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
        
        // 注册全局伤害事件监听
        GameRoot.GetInstance().RegisterUpdateMethod(CheckDamageEvents);
    }
    
    private void OnButtonEClick()
    {
        // 检查是否在冷却中
        if(currentCooldownE <= 0)
        {
            // 播放点击音效
            AudioManager.PlayClick();
            
            // 开始技能冷却
            currentCooldownE = cooldownE;
            buttonE.interactable = false;
            
            // 这里可以添加技能E的具体逻辑
            Debug.Log("技能E被使用");
        }
    }
    
    private void OnButtonQClick()
    {
        // 检查是否在冷却中
        if(currentCooldownQ <= 0)
        {
            // 播放点击音效
            AudioManager.PlayClick();
            
            // 开始技能冷却
            currentCooldownQ = cooldownQ;
            buttonQ.interactable = false;
            
            // 这里可以添加技能Q的具体逻辑
            Debug.Log("技能Q被使用");
        }
    }
    
    // 受到伤害
    public void TakeDamage(float damage)
    {
        lastDamageTime = Time.time;
        
        // 先扣护盾
        if(currentShield > 0)
        {
            if(currentShield >= damage)
            {
                currentShield -= damage;
                damage = 0;
            }
            else
            {
                damage -= currentShield;
                currentShield = 0;
            }
        }
        
        // 再扣血量
        if(damage > 0 && currentHealth > 0)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
        }
        
        // 更新目标百分比
        targetHealthPercent = currentHealth / maxHealth;
        targetShieldPercent = currentShield / maxShield;
        
        Debug.Log($"受到伤害: {damage}, 当前血量: {currentHealth}, 当前护盾: {currentShield}");
        
        // 检查是否死亡
        if(currentHealth <= 0)
        {
            OnDeath();
        }
    }
    
    // 回复血量
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        targetHealthPercent = currentHealth / maxHealth;
    }
    
    // 回复护盾
    public void RestoreShield(float amount)
    {
        currentShield = Mathf.Min(maxShield, currentShield + amount);
        targetShieldPercent = currentShield / maxShield;
    }
    
    // 死亡处理
    private void OnDeath()
    {
        Debug.Log("玩家死亡！");
        // 这里可以添加死亡逻辑，比如重新开始游戏或显示死亡面板
    }
    
    // 更新血量 UI
    private void UpdateHealthUI()
    {
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
        
        if(shieldText != null)
        {
            shieldText.text = Mathf.Ceil(currentShield).ToString();
        }
    }
    
    // 更新武器显示（三角形旋转切换）
    private void UpdateWeaponDisplay()
    {
        if(weapon1Rect == null || weapon2Rect == null || weapon3Rect == null)
            return;
        
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
            weapon1Rect.localScale = (targetPos == topPositionIndex) ? Vector3.one * 1.1f : Vector3.one;
            weapon1IsTop = (targetPos == topPositionIndex);
        }
        
        // 更新武器 2：放在目标位置，只有在顶部位置时放大
        if(weapon2Rect != null)
        {
            int targetPos = targetIndices[1];
            weapon2Rect.localPosition = trianglePoints[targetPos];
            weapon2Rect.localScale = (targetPos == topPositionIndex) ? Vector3.one * 1.1f : Vector3.one;
            weapon2IsTop = (targetPos == topPositionIndex);
        }
        
        // 更新武器 3：放在目标位置，只有在顶部位置时放大
        if(weapon3Rect != null)
        {
            int targetPos = targetIndices[2];
            weapon3Rect.localPosition = trianglePoints[targetPos];
            weapon3Rect.localScale = (targetPos == topPositionIndex) ? Vector3.one * 1.1f : Vector3.one;
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
    
    // 检查伤害事件（用于测试）
    private void CheckDamageEvents()
    {
        // 按H键受到伤害（测试用）
        if(Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(20f);
        }
        
        // 按J键回复血量（测试用）
        if(Input.GetKeyDown(KeyCode.J))
        {
            Heal(15f);
        }
        
        // 按K键回复护盾（测试用）
        if(Input.GetKeyDown(KeyCode.K))
        {
            RestoreShield(20f);
        }
    }
    
    private void Update()
    {
        // 武器切换输入（鼠标滚轮）
        if(currentWeaponCooldown <= 0)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            
            if(scrollInput > 0f)
            {
                // 向上滚动：逆时针切换（0->2->1->0）
                currentWeaponIndex = (currentWeaponIndex + 2) % 3;
                currentWeaponCooldown = weaponSwitchCooldown;
                UpdateWeaponDisplay();
                Debug.Log($"武器切换到：{currentWeaponIndex + 1}");
            }
            else if(scrollInput < 0f)
            {
                // 向下滚动：顺时针切换（0->1->2->0）
                currentWeaponIndex = (currentWeaponIndex + 1) % 3;
                currentWeaponCooldown = weaponSwitchCooldown;
                UpdateWeaponDisplay();
                Debug.Log($"武器切换到：{currentWeaponIndex + 1}");
            }
        }
        
        // 检测键盘输入
        if(Input.GetKeyDown(KeyCode.E))
        {
            OnButtonEClick();
        }
        
        if(Input.GetKeyDown(KeyCode.Q))
        {
            OnButtonQClick();
        }
        
        // 更新技能E的冷却
        if(currentCooldownE > 0)
        {
            currentCooldownE -= Time.deltaTime;
            
            // 更新冷却文本
            if(cooldownTextE != null)
            {
                cooldownTextE.text = Mathf.Ceil(currentCooldownE).ToString();
                cooldownTextE.gameObject.SetActive(true);
            }
            
            // 冷却结束
            if(currentCooldownE <= 0)
            {
                buttonE.interactable = true;
                if(cooldownTextE != null)
                {
                    cooldownTextE.gameObject.SetActive(false);
                }
            }
        }
        
        // 更新技能Q的冷却
        if(currentCooldownQ > 0)
        {
            currentCooldownQ -= Time.deltaTime;
            
            // 更新冷却文本
            if(cooldownTextQ != null)
            {
                cooldownTextQ.text = Mathf.Ceil(currentCooldownQ).ToString();
                cooldownTextQ.gameObject.SetActive(true);
            }
            
            // 冷却结束
            if(currentCooldownQ <= 0)
            {
                buttonQ.interactable = true;
                if(cooldownTextQ != null)
                {
                    cooldownTextQ.gameObject.SetActive(false);
                }
            }
        }
        
        // 更新血量 UI
        UpdateHealthUI();
        
        // 更新武器切换冷却
        if(currentWeaponCooldown > 0)
        {
            currentWeaponCooldown -= Time.deltaTime;
        }
        
        // 自动回复逻辑
        if(Time.time - lastDamageTime >= regenDelay)
        {
            if(currentHealth < maxHealth)
            {
                Heal(healthRegenRate * Time.deltaTime);
            }
            if(currentShield < maxShield)
            {
                RestoreShield(shieldRegenRate * Time.deltaTime);
            }
        }
    }
    
    private void BackButtonClick()
    {
        // 播放点击音效
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
        // 取消注册Update方法
        GameRoot.GetInstance().UnregisterUpdateMethod(Update);
        base.OnDestroy();
    }
   
}
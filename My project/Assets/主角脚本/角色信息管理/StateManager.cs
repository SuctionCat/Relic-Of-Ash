using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;

    [Header("角色状态")]
    public float currentHealth;
    public float currentShield;
    public float maxShield;
    private int currentWeaponIndex;
    private string currentWeaponName;

    [Header("技能冷却时间")]
    public float qCooldownTime = 2f;
    public float eCooldownTime = 3f;
    
    private float qCooldownRemaining;
    private float eCooldownRemaining;

    public delegate void WeaponIndexChanged(int newIndex);
    public event WeaponIndexChanged OnWeaponIndexChanged;

    public delegate void PlayerDeadHandler();
    public event PlayerDeadHandler OnPlayerDead;

    public delegate void PlayerRevivedHandler();
    public event PlayerRevivedHandler OnPlayerRevived;

    private Dictionary<int, string> weaponNames = new Dictionary<int, string>
    {
        { 0, "Sword" },
        { 1, "Katana" },
        { 2, "Stick" }
    };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeFromPlayerMemento();
        qCooldownRemaining = 0f;
        eCooldownRemaining = 0f;

        if (GameRoot.GetInstance() != null)
        {
            GameRoot.GetInstance().RegisterStateManager();
        }
    }
    
    void Update()
    {
        UpdateCooldowns();
    }
    
    private void UpdateCooldowns()
    {
        if (qCooldownRemaining > 0)
        {
            qCooldownRemaining -= Time.deltaTime;
            if (qCooldownRemaining < 0) qCooldownRemaining = 0;
        }
        
        if (eCooldownRemaining > 0)
        {
            eCooldownRemaining -= Time.deltaTime;
            if (eCooldownRemaining < 0) eCooldownRemaining = 0;
        }
    }
    
    private void InitializeFromPlayerMemento()
    {
        if (PlayerMemento.instance != null)
        {
            currentHealth = PlayerMemento.instance.GetInitialHealth();
            currentShield = PlayerMemento.instance.GetInitialShield();
            maxShield = PlayerMemento.instance.GetInitialMaxShield();
            Debug.Log($"StateManager: 已从 PlayerMemento 初始化生命值为 {currentHealth}，护盾值为 {currentShield}");
        }
        else
        {
            currentHealth = 1000f;
            currentShield = 200f;
            maxShield = 200f;
            Debug.LogWarning("PlayerMemento 未找到，使用默认生命值和护盾值");
        }
    }

    public void SetHealth(float health)
    {
        bool wasDead = currentHealth <= 0f;
        currentHealth = health;
        Debug.Log($"StateManager: 生命值已更新为 {currentHealth}");
        
        // 如果玩家之前死亡且现在生命值大于0，则触发复活事件
        if (wasDead && currentHealth > 0f)
        {
            NotifyPlayerRevived();
        }
    }
    
    public void NotifyPlayerRevived()
    {
        Debug.Log("StateManager: 玩家已复活");
        OnPlayerRevived?.Invoke();
    }

    public void SetShield(float shield)
    {
        currentShield = Mathf.Clamp(shield, 0f, maxShield);
        Debug.Log($"StateManager: 护盾值已更新为 {currentShield}");
    }

    public void AddShield(float amount)
    {
        currentShield = Mathf.Clamp(currentShield + amount, 0f, maxShield);
        Debug.Log($"StateManager: 护盾值增加 {amount}，当前护盾值为 {currentShield}");
    }

    public float GetShield()
    {
        return currentShield;
    }

    public float GetMaxShield()
    {
        return maxShield;
    }

    public void SetWeaponIndex(int index)
    {
        currentWeaponIndex = index;
        currentWeaponName = weaponNames.ContainsKey(index) ? weaponNames[index] : "Unknown";
        Debug.Log($"StateManager: 当前武器已更新为 {currentWeaponName} (索引: {currentWeaponIndex})");
        
        OnWeaponIndexChanged?.Invoke(index);
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public int GetWeaponIndex()
    {
        return currentWeaponIndex;
    }

    public string GetWeaponName()
    {
        return currentWeaponName;
    }

    public bool IsQReady()
    {
        return qCooldownRemaining <= 0f;
    }

    public bool IsEReady()
    {
        return eCooldownRemaining <= 0f;
    }

    public void StartQCooldown()
    {
        qCooldownRemaining = qCooldownTime;
        Debug.Log($"StateManager: Q技能开始冷却，冷却时间 {qCooldownTime}秒");
    }

    public void StartECooldown()
    {
        eCooldownRemaining = eCooldownTime;
        Debug.Log($"StateManager: E技能开始冷却，冷却时间 {eCooldownTime}秒");
    }

    public float GetQCooldownRemaining()
    {
        return qCooldownRemaining;
    }

    public float GetECooldownRemaining()
    {
        return eCooldownRemaining;
    }

    public void NotifyPlayerDead()
    {
        Debug.Log("StateManager: 收到玩家死亡通知");
        OnPlayerDead?.Invoke();
    }
}
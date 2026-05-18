using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;

    [Header("角色状态")]
    public float currentHealth;
    private int currentWeaponIndex;
    private string currentWeaponName;

    public delegate void WeaponIndexChanged(int newIndex);
    public event WeaponIndexChanged OnWeaponIndexChanged;

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeFromPlayerMemento();
    }
    
    private void InitializeFromPlayerMemento()
    {
        if (PlayerMemento.instance != null)
        {
            currentHealth = PlayerMemento.instance.GetInitialHealth();
            Debug.Log($"StateManager: 已从 PlayerMemento 初始化生命值为 {currentHealth}");
        }
        else
        {
            currentHealth = 1000f;
            Debug.LogWarning("PlayerMemento 未找到，使用默认生命值");
        }
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        Debug.Log($"StateManager: 生命值已更新为 {currentHealth}");
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;

    [Header("角色状态")]
    private float currentHealth;
    private int currentWeaponIndex;
    private string currentWeaponName;

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
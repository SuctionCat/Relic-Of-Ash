using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMemento : MonoBehaviour
{
    public static PlayerMemento instance;

    [Header("初始状态数据")]
    public float initialMaxHealth = 1000f;
    private float initialHealth;
    private int initialWeaponIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SaveInitialState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveInitialState()
    {
        initialHealth = initialMaxHealth;
        initialWeaponIndex = 0;
        Debug.Log("PlayerMemento: 已保存初始状态 - 生命值: " + initialHealth + ", 武器索引: " + initialWeaponIndex);
    }

    public float GetInitialHealth()
    {
        return initialHealth;
    }

    public int GetInitialWeaponIndex()
    {
        return initialWeaponIndex;
    }

    public float GetInitialMaxHealth()
    {
        return initialMaxHealth;
    }
}
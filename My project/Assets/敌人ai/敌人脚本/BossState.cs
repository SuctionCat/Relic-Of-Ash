using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private float currentHealth;
    private float maxHealth;

    // Boss名称
    public string bossName = "Boss";

    // 是否已经初始化
    private bool hasInitialized = false;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            maxHealth = enemyHealth.currentHealth;
            currentHealth = maxHealth;
            Debug.Log("Boss初始生命值: " + currentHealth);
        }
    }

    // 初始化BossBar
    public void InitializeBossBar()
    {
        if (hasInitialized) return;
        hasInitialized = true;

        // 推送BossBar面板
        BossBarPanel bossBarPanel = new BossBarPanel();
        GameRoot.GetInstance().UIManager_Root.Push(bossBarPanel);

        // 设置Boss数据
        if (enemyHealth != null)
        {
            maxHealth = enemyHealth.currentHealth;
            currentHealth = maxHealth;
        }

        // 延迟一帧确保面板已创建
        StartCoroutine(SetBossDataDelayed(bossBarPanel));
    }

    private IEnumerator SetBossDataDelayed(BossBarPanel bossBarPanel)
    {
        yield return null;
        bossBarPanel.SetBossData(maxHealth, currentHealth);
        Debug.Log("BossBar已初始化: 生命值: " + currentHealth + "/" + maxHealth);
    }

    public void OnHealthChanged(float newHealth)
    {
        currentHealth = newHealth;
        Debug.Log("Boss生命值变化: " + currentHealth);

        // 通知BossBar更新
        UpdateBossBar();
    }

    private void UpdateBossBar()
    {
        // 查找BossBarPanel并更新
        foreach (var panel in GameRoot.GetInstance().UIManager_Root.stack_ui)
        {
            BossBarPanel bossBar = panel as BossBarPanel;
            if (bossBar != null)
            {
                bossBar.UpdateHealth(currentHealth);
                break;
            }
        }
    }

    // 获取当前生命值
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // 获取最大生命值
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // 获取Boss名称
    public string GetBossName()
    {
        return bossName;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private float currentHealth;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            currentHealth = enemyHealth.currentHealth;
            Debug.Log("Boss初始生命值: " + currentHealth);
        }
    }

    public void OnHealthChanged(float newHealth)
    {
        currentHealth = newHealth;
        Debug.Log("Boss生命值变化: " + currentHealth);
    }
}

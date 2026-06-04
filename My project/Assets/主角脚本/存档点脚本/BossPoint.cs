using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPoint : MonoBehaviour
{
    // 关联的Boss对象
    public GameObject bossObject;

    // 是否已经触发
    private bool hasTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        // 如果没有设置boss对象，尝试在场景中查找
        if (bossObject == null)
        {
            bossObject = GameObject.FindGameObjectWithTag("Boss");
            if (bossObject != null)
            {
                Debug.Log("BossPoint: 自动找到Boss对象: " + bossObject.name);
            }
            else
            {
                Debug.LogWarning("BossPoint: 未找到Boss对象，请在Inspector中设置");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家，且尚未触发
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("BossPoint: 玩家进入Boss区域");

            // 尝试获取BossState组件并初始化血条
            if (bossObject != null)
            {
                BossState bossState = bossObject.GetComponent<BossState>();
                if (bossState != null)
                {
                    bossState.InitializeBossBar();
                    Debug.Log("BossPoint: 已初始化Boss血条");
                }
                else
                {
                    Debug.LogError("BossPoint: Boss对象上没有BossState组件");
                }
            }
            else
            {
                Debug.LogError("BossPoint: Boss对象为空");
            }

            // 可选：禁用触发器，防止重复触发
            GetComponent<Collider>().enabled = false;
        }
    }

    // 重置触发器（用于测试）
    public void ResetTrigger()
    {
        hasTriggered = false;
        GetComponent<Collider>().enabled = true;
    }
}
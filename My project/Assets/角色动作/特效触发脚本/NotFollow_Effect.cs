using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectController : MonoBehaviour
{
    [Header("特效管理设置")]
    [Tooltip("在这里配置所有的特效组及其对应的Key名称")]
    public List<EffectGroup> effectGroups = new List<EffectGroup>();

    // 用于快速查找的内部字典
    private Dictionary<string, EffectGroup> effectDictionary = new Dictionary<string, EffectGroup>();

    [Header("通用设置")]
    public Transform weaponPoint; // 武器发射点

    // 定义特效组结构
    [System.Serializable]
    public class EffectGroup
    {
        public string effectKey;                  // 特效组的唯一标识名称（例如："HeavySlash"）
        
        [Tooltip("该组内包含的所有特效，会同时播放")]
        public List<ParticleSystem> effectPrefabs = new List<ParticleSystem>(); // 这里变成了列表
    }

    void Start()
    {
        // 初始化字典
        effectDictionary.Clear();
        foreach (var group in effectGroups)
        {
            if (!string.IsNullOrEmpty(group.effectKey) && group.effectPrefabs.Count > 0)
            {
                if (effectDictionary.ContainsKey(group.effectKey))
                {
                    Debug.LogWarning($"[AttackEffectController] 检测到重复的 Effect Key: {group.effectKey}，将使用后者覆盖。");
                    effectDictionary[group.effectKey] = group;
                }
                else
                {
                    effectDictionary.Add(group.effectKey, group);
                }
            }
        }
    }

    /// <summary>
    /// 由动画事件调用的函数
    /// </summary>
    /// <param name="keyName">在Inspector中设置的特效Key名字</param>
    public void TriggerEffect(string keyName)
    {
        // 1. 查找对应的特效组
        if (effectDictionary.TryGetValue(keyName, out EffectGroup targetGroup))
        {
            // 2. 确定位置和旋转 (所有特效共享同一个生成点)
            Vector3 spawnPos = weaponPoint != null ? weaponPoint.position : transform.position;
            Quaternion spawnRot = transform.rotation;

            // 3. 遍历列表，实例化该组内的所有特效
            foreach (var prefab in targetGroup.effectPrefabs)
            {
                if (prefab != null)
                {
                    // 在世界坐标生成，无父物体
                    GameObject effectInstance = Instantiate(prefab.gameObject, spawnPos, spawnRot);
                    
                    // 播放粒子
                    ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.Play();
                    }
                    else
                    {
                        // 兼容处理：如果预制体根节点没有PS，尝试找子物体
                        ParticleSystem[] childPS = effectInstance.GetComponentsInChildren<ParticleSystem>();
                        foreach(var child in childPS) child.Play();
                    }

                    // 自动销毁
                    // 注意：如果组内特效时长差异巨大，这里简单的2秒销毁可能不够完美
                    // 完美做法是读取所有特效的 duration 取最大值，但通常2-3秒足够通用
                    Destroy(effectInstance, 1.5f); 
                }
            }
        }
        else
        {
            Debug.LogWarning($"[AttackEffectController] 未找到名为 '{keyName}' 的特效组，请检查拼写。");
        }
    }
}
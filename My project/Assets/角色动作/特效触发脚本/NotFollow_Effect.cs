using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectController : MonoBehaviour
{
    [Header("特效管理设置")]
    [Tooltip("在这里配置所有的特效组及其对应的Key名称")]
    public List<EffectGroup> effectGroups = new List<EffectGroup>();

    private Dictionary<string, EffectGroup> effectDictionary = new Dictionary<string, EffectGroup>();

    [Header("通用设置")]
    public Transform weaponPoint; // 武器发射点

    [Header("发射设置")]
    [Tooltip("特效移动的速度（单位：米/秒）")]
    public float launchSpeed = 10f;
    
    [Tooltip("特效持续飞行的时间（秒）")]
    public float flightDuration = 2.0f;

    [Tooltip("发射时的高度偏移（解决特效在脚底下的问题）")]
    public float launchHeightOffset = 1.0f;

    [Tooltip("发射位置距离玩家/武器点的水平距离（制造发射感）")] // --- 新增参数
    public float spawnDistance = 2.0f;

    [System.Serializable]
    public class EffectGroup
    {
        public string effectKey;
        [Tooltip("该组内包含的所有特效")]
        public List<ParticleSystem> effectPrefabs = new List<ParticleSystem>();
    }

    void Start()
    {
        effectDictionary.Clear();
        foreach (var group in effectGroups)
        {
            if (!string.IsNullOrEmpty(group.effectKey) && group.effectPrefabs.Count > 0)
            {
                if (effectDictionary.ContainsKey(group.effectKey))
                {
                    effectDictionary[group.effectKey] = group;
                }
                else
                {
                    effectDictionary.Add(group.effectKey, group);
                }
            }
        }
    }

    public void TriggerEffect(string keyName)
    {
        if (effectDictionary.TryGetValue(keyName, out EffectGroup targetGroup))
        {
            // 1. 获取基础位置（武器点或玩家中心）
            Vector3 basePos = weaponPoint != null ? weaponPoint.position : transform.position;
            
            // 2. 获取玩家朝向
            Quaternion baseRot = transform.rotation;

            // --- 修改点：计算最终生成位置 ---
            // 基础位置 + 向上抬高 + 向前推移
            // baseRot * Vector3.forward 获取的是“玩家前方”的向量
            Vector3 spawnPos = basePos + (Vector3.up * launchHeightOffset) + (baseRot * Vector3.forward * spawnDistance);

            foreach (var prefab in targetGroup.effectPrefabs)
            {
                if (prefab != null)
                {
                    // 3. 视觉旋转：依然旋转180度，让特效背对玩家
                    Quaternion finalRot = baseRot * Quaternion.Euler(0, 180, 0);
                    
                    // 实例化特效（使用计算好的前方位置）
                    GameObject effectInstance = Instantiate(prefab.gameObject, spawnPos, finalRot);

                    // 播放粒子
                    PlayEffect(effectInstance);

                    // 4. 移动方向：依然是玩家的正前方
                    Vector3 direction = baseRot * Vector3.forward;

                    StartCoroutine(MoveEffectRoutine(effectInstance, direction, flightDuration));
                }
            }
        }
        else
        {
            Debug.LogWarning($"[AttackEffectController] 未找到名为 '{keyName}' 的特效组");
        }
    }

    private void PlayEffect(GameObject obj)
    {
        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();
        else
        {
            ParticleSystem[] childPS = obj.GetComponentsInChildren<ParticleSystem>();
            foreach (var child in childPS) child.Play();
        }
    }

    private IEnumerator MoveEffectRoutine(GameObject effectObj, Vector3 moveDirection, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 移动坐标
            effectObj.transform.Translate(moveDirection * launchSpeed * Time.deltaTime, Space.World);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(effectObj);
    }
}
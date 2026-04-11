using System;
using System.Collections.Generic;
using UnityEngine;

public class ComboEffectManager : MonoBehaviour
{
    [Serializable]
    public class EffectConfig
    {
        [Tooltip("唯一标识符，例如 'Attack01_Loop', 'Shield_Active'")]
        public string effectKey; 
        
        [Tooltip("对应的特效列表")]
        public List<ParticleSystem> effects = new List<ParticleSystem>();
    }

    public List<EffectConfig> configList = new List<EffectConfig>();

    // --- 1. 播放特效 ---
    // 动画事件调用：在第 X 帧播放
    public void PlayEffect(string key)
    {
        foreach (var config in configList)
        {
            if (config.effectKey == key)
            {
                foreach (var effect in config.effects)
                {
                    if (effect != null)
                    {
                        effect.Clear(); // 清除旧粒子，防止叠加
                        effect.Play();
                    }
                }
                return;
            }
        }
        Debug.LogWarning("未找到特效标识符 (Play): " + key);
    }

    // --- 2. 停止特效 ---
    // 动画事件调用：在第 Y 帧停止
    public void StopEffect(string key)
    {
        foreach (var config in configList)
        {
            if (config.effectKey == key)
            {
                foreach (var effect in config.effects)
                {
                    if (effect != null)
                    {
                        // 停止发射新粒子，并让现有粒子自然消失（平滑停止）
                        effect.Stop(); 
                    }
                }
                return;
            }
        }
        Debug.LogWarning("未找到特效标识符 (Stop): " + key);
    }
}
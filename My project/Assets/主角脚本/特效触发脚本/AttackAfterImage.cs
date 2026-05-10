using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAfterImage : MonoBehaviour
{
    [Header("残影设置")]
    public GameObject afterImagePrefab; // 残影的预制体
    public SkinnedMeshRenderer characterRenderer; // 角色的 SkinnedMeshRenderer 组件
    public float fadeDuration = 0.3f; // 残影淡出持续时间

    // 新增：自定义的残影材质
    public Material customAfterImageMaterial; // 用于自定义材质的变量

    // 由动画事件调用的方法
    public void SpawnAfterImage()
    {
        StartCoroutine(GenerateAfterImage());
    }

    private IEnumerator GenerateAfterImage()
    {
        // 1. 创建一个空的游戏对象作为残影容器
        GameObject afterImageGO = new GameObject("AfterImage");
        afterImageGO.transform.position = transform.position;
        afterImageGO.transform.rotation = transform.rotation;
        afterImageGO.transform.localScale = transform.localScale;

        // 2. 烘焙当前帧的网格，以精确复制角色姿态
        Mesh bakedMesh = new Mesh();
        characterRenderer.BakeMesh(bakedMesh);

        // 3. 为残影添加 MeshFilter 和 MeshRenderer 组件
        MeshFilter meshFilter = afterImageGO.AddComponent<MeshFilter>();
        meshFilter.mesh = bakedMesh;

        MeshRenderer meshRenderer = afterImageGO.AddComponent<MeshRenderer>();
        
        // 4. 使用自定义材质球而不是角色材质
        if (customAfterImageMaterial != null)
        {
            meshRenderer.material = customAfterImageMaterial; // 使用自定义材质
        }
        else
        {
            // 如果没有指定自定义材质，则使用角色的原始材质
            meshRenderer.material = new Material(characterRenderer.sharedMaterial);
        }

        // 5. 开始淡出协程
        float elapsedTime = 0f;
        Material afterImageMat = meshRenderer.material;
        Color originalColor = afterImageMat.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // 计算透明度，从1逐渐变为0
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            afterImageMat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 6. 淡出完成后，销毁残影对象
        Destroy(afterImageGO);
    }
}

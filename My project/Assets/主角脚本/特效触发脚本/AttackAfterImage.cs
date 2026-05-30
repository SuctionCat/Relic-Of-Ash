using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAfterImage : MonoBehaviour
{
    [Header("残影设置")]
    public GameObject afterImagePrefab;//残影预制体，用于创建残影实例
    public SkinnedMeshRenderer characterRenderer;//角色网格渲染器，用于获取角色网格信息
    public float fadeDuration = 0.3f;//残影淡出时间
    public Material customAfterImageMaterial;//自定义残影材质，用于自定义残影的外观

    private bool shouldSpawnAfterImage = false;

    public void SpawnAfterImage()
    {
        shouldSpawnAfterImage = true;
    }

    private void LateUpdate()
    {
        if (shouldSpawnAfterImage)
        {
            shouldSpawnAfterImage = false;
            StartCoroutine(GenerateAfterImage());
        }
    }

    private IEnumerator GenerateAfterImage()
    {
        GameObject afterImageGO = new GameObject("AfterImage");
        afterImageGO.transform.position = transform.position;
        afterImageGO.transform.rotation = transform.rotation;
        afterImageGO.transform.localScale = transform.localScale;

        Mesh bakedMesh = new Mesh();
        characterRenderer.BakeMesh(bakedMesh);

        MeshFilter meshFilter = afterImageGO.AddComponent<MeshFilter>();
        meshFilter.mesh = bakedMesh;

        MeshRenderer meshRenderer = afterImageGO.AddComponent<MeshRenderer>();

        if (customAfterImageMaterial != null)
        {
            meshRenderer.material = customAfterImageMaterial;
        }
        else
        {
            meshRenderer.material = new Material(characterRenderer.sharedMaterial);
        }

        float elapsedTime = 0f;
        Material afterImageMat = meshRenderer.material;
        Color originalColor = afterImageMat.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            afterImageMat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(afterImageGO);
    }
}

using UnityEngine;

public class BindSwordToCharacter : MonoBehaviour
{
    public GameObject character; // SK_M_ROGUE_02 Violet
    public GameObject sword; // 9CG_Sword

    public void BindSkeleton()
    {
        if (character == null || sword == null)
        {
            Debug.LogError("Character or sword is not assigned!");
            return;
        }

        // 1. 移除角色原有骨骼
        RemoveOriginalBones();

        // 2. 复制剑的骨骼到角色
        CopySwordBones();

        // 3. 更新角色的SkinnedMeshRenderer组件
        UpdateSkinnedMeshRenderers();

        Debug.Log("Skeleton binding completed successfully!");
    }

    private void RemoveOriginalBones()
    {
        // 查找并移除角色的原有骨骼
        Transform[] allChildren = character.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            // 假设原有骨骼名称包含"bone"、"spine"、"arm"等关键词
            if (child.name.ToLower().Contains("bone") || 
                child.name.ToLower().Contains("spine") || 
                child.name.ToLower().Contains("arm") ||
                child.name.ToLower().Contains("leg") ||
                child.name.ToLower().Contains("hand") ||
                child.name.ToLower().Contains("foot") ||
                child.name.ToLower().Contains("head") ||
                child.name.ToLower().Contains("neck"))
            {
                // 保存子对象引用
                Transform[] grandChildren = new Transform[child.childCount];
                for (int i = 0; i < child.childCount; i++)
                {
                    grandChildren[i] = child.GetChild(i);
                }

                // 移除骨骼，但保留非骨骼子对象
                foreach (Transform grandChild in grandChildren)
                {
                    if (!IsBone(grandChild))
                    {
                        grandChild.parent = child.parent;
                    }
                }

                // 销毁骨骼对象
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private bool IsBone(Transform transform)
    {
        // 判断是否为骨骼
        string name = transform.name.ToLower();
        return name.Contains("bone") || 
               name.Contains("spine") || 
               name.Contains("arm") ||
               name.Contains("leg") ||
               name.Contains("hand") ||
               name.Contains("foot") ||
               name.Contains("head") ||
               name.Contains("neck");
    }

    private void CopySwordBones()
    {
        // 复制剑的骨骼层级到角色
        Transform[] swordBones = sword.GetComponentsInChildren<Transform>();
        foreach (Transform bone in swordBones)
        {
            if (bone != sword.transform) // 跳过根对象
            {
                GameObject newBone = Instantiate(bone.gameObject, character.transform);
                newBone.name = bone.name;
                newBone.transform.localPosition = bone.transform.localPosition;
                newBone.transform.localRotation = bone.transform.localRotation;
                newBone.transform.localScale = bone.transform.localScale;
            }
        }
    }

    private void UpdateSkinnedMeshRenderers()
    {
        // 获取角色的所有SkinnedMeshRenderer组件
        SkinnedMeshRenderer[] renderers = character.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            // 获取新的骨骼数组
            Transform[] newBones = new Transform[renderer.bones.Length];
            for (int i = 0; i < renderer.bones.Length; i++)
            {
                string boneName = renderer.bones[i].name;
                Transform newBone = character.transform.Find(boneName);
                if (newBone != null)
                {
                    newBones[i] = newBone;
                }
                else
                {
                    Debug.LogWarning($"Bone {boneName} not found in new skeleton!");
                }
            }

            // 更新骨骼数组
            renderer.bones = newBones;

            // 设置根骨骼
            string rootBoneName = renderer.rootBone != null ? renderer.rootBone.name : ""; 
            if (!string.IsNullOrEmpty(rootBoneName))
            {
                Transform newRootBone = character.transform.Find(rootBoneName);
                if (newRootBone != null)
                {
                    renderer.rootBone = newRootBone;
                }
            }
        }
    }
}

using UnityEngine;

public class Effect_Damage : MonoBehaviour
{
    [Header("攻击设置")]
    public int damage = 20;
    public float knockback = 10f;

    [Header("检测范围设置")]
    // 射线的长度（剑气向前延伸的检测距离）
    public float checkDistance = 1.0f;
    
    // 【新增】剑气的宽度（对应 BoxCast 的半宽，比如剑气宽1米，这里填0.5）
    public float beamWidth = 1.0f; 
    
    // 【新增】剑气的高度（通常设为角色的半身高度即可）
    public float beamHeight = 1.0f;

    void Update()
    {
        CheckHit();
    }

    void CheckHit()
    {
        // 定义盒子的半尺寸 (Vector3.right * 宽, Vector3.up * 高, Vector3.forward * 深)
        // 注意：这里的深度我们给一个很小的值，主要靠移动来扫描
        Vector3 halfExtents = new Vector3(beamWidth / 2, beamHeight / 2, 0.1f);
        
        RaycastHit hit;
        
        // 使用 BoxCast 代替 Raycast
        // 参数：中心点, 半尺寸, 方向, 命中信息, 距离
        if (Physics.BoxCast(transform.position, halfExtents, transform.forward, out hit, transform.rotation, checkDistance))
        {
            // 绘制调试框，方便你在 Scene 视图看到检测范围（红色盒子）
            Debug.DrawRay(transform.position, transform.forward * checkDistance, Color.red);
            
            // 检查标签
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy == null)
                {
                    enemy = hit.collider.GetComponentInParent<EnemyHealth>();
                }

                if (enemy != null)
                {
                    enemy.TakeHit(damage, knockback);
                    Debug.Log($"宽剑气击中了：{hit.collider.name}");
                    
                    // 击中后销毁
                    //Destroy(gameObject); 
                }
            }
        }
    }
    
    // 辅助绘图，让盒子在编辑器里可见
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f); // 半透明红色
        Vector3 center = transform.position + transform.forward * (checkDistance / 2);
        Vector3 size = new Vector3(beamWidth, beamHeight, checkDistance);
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, size);
    }
}
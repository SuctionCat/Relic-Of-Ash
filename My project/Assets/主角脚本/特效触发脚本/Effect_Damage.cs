using UnityEngine;

public class Effect_Damage : MonoBehaviour
{
    [Header("攻击设置")]
    public int damage = 20;
    public float knockback = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyHealth>();
            }

            if (enemy != null)
            {
                enemy.TakeHit(damage, knockback, this.transform, false);
                Debug.Log($"剑气击中了：{other.name}");
            }
        }
    }
}
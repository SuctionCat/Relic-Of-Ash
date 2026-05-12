using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateCheckPoint();
        }
    }

    public void ActivateCheckPoint()
    {
        if (isActive) return;
        isActive = true;
        Debug.Log($"存档点已激活！当前位置：" + transform.position);
        GameManager.Instance.SetCheckpoint(this);
    }
}

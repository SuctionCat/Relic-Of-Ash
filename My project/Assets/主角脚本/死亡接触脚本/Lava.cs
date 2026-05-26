using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (StateManager.instance != null)
            {
                StateManager.instance.SetHealth(0f);
                Debug.Log("Lava: 玩家触碰到岩浆，生命值已设置为0");
            }
        }
    }
}

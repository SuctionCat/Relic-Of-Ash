using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CheckPoint LatestCheckPoint;
    private Vector3 spawnPoint;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeSpawnPoint();
    }
    
    private void InitializeSpawnPoint()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && spawnPoint == Vector3.zero)
        {
            spawnPoint = player.transform.position;
            Debug.Log("GameManager: 已初始化重生位置为玩家初始位置: " + spawnPoint);
        }
    }

    public void SetCheckpoint(CheckPoint checkpoint)
    {
        if (LatestCheckPoint != null)
        {
            LatestCheckPoint.isActive = false;
        }
        LatestCheckPoint = checkpoint;
        spawnPoint = checkpoint.transform.position;
    }

    public void PlayerRespawn()
    {
        Debug.Log("玩家重生! 重生位置: " + spawnPoint);
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            Animator animator = player.GetComponent<Animator>();
            
            if (controller != null)
            {
                controller.enabled = false;
            }
            
            if (animator != null)
            {
                animator.applyRootMotion = false;
            }
            
            if (spawnPoint == Vector3.zero)
            {
                spawnPoint = player.transform.position;
            }
            
            player.transform.position = spawnPoint;
            
            StartCoroutine(DelayedResetPlayerState(player));
        }
        else
        {
            Debug.LogWarning("GameManager: 未找到玩家对象，无法重生");
        }
    }
    
    private IEnumerator DelayedResetPlayerState(GameObject player)
    {
        yield return new WaitForEndOfFrame();
        
        player.transform.position = spawnPoint;
        
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = true;
        }
        
        ResetPlayerState(player);
    }

    private void ResetPlayerState(GameObject player)
    {
        if (PlayerMemento.instance != null && StateManager.instance != null)
        {
            float initialHealth = PlayerMemento.instance.GetInitialHealth();
            int initialWeaponIndex = PlayerMemento.instance.GetInitialWeaponIndex();

            StateManager.instance.SetHealth(initialHealth);
            StateManager.instance.SetWeaponIndex(initialWeaponIndex);

            if (player != null)
            {
                BeAttack beAttack = player.GetComponent<BeAttack>();
                if (beAttack != null)
                {
                    beAttack.Health = initialHealth;
                    beAttack.ResetDeadState();
                }
                
                Animator animator = player.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Rebind();
                    animator.Update(0);
                    animator.applyRootMotion = true;
                }
            }

            Debug.Log("GameManager: 已重置玩家状态 - 生命值: " + initialHealth + ", 武器索引: " + initialWeaponIndex);
        }
        else
        {
            Debug.LogWarning("GameManager: PlayerMemento或StateManager未找到，无法重置玩家状态");
        }
    }
}

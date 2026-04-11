using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float speed = 3f;
    public float chaseSpeed = 5f;
    public float detectionRadius = 10f;
    public Transform player;
    private int currentPointIndex = 0;
    private bool isChasing = false;

    // Start is called before the first frame update
    void Start()
    {
        if (patrolPoints == null || patrolPoints.Length < 2)
        {
            Debug.LogError("请在Inspector中设置至少两个巡逻点");
        }
        if (player == null)
        {
            Debug.LogError("请在Inspector中设置玩家对象");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerDistance();
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void CheckPlayerDistance()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isChasing = distanceToPlayer < detectionRadius;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length < 2)
            return;

        Transform target = patrolPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        if (player == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }
}

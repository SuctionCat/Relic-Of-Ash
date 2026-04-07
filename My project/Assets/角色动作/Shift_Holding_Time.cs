using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastRun_Time : MonoBehaviour
{
    private float shiftHoldTime = 0f;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            shiftHoldTime += Time.deltaTime;
        }
        else
        {
            shiftHoldTime = 0f;
        }

        if (animator != null)
        {
            animator.SetFloat("Shift_Time", shiftHoldTime);
        }
    }
}

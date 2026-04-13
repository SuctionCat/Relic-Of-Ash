using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Run_Speed : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        animator.SetBool("isRunning", isRunning);
        bool isWHolding = Input.GetKey(KeyCode.W);
        //animator.SetBool("W_holding", isWHolding);
        bool isAHolding = Input.GetKey(KeyCode.A);
        //animator.SetBool("A_holding", isAHolding);
        bool isSHolding = Input.GetKey(KeyCode.S);
        //animator.SetBool("S_holding", isSHolding);
        bool isDHolding = Input.GetKey(KeyCode.D);
        //animator.SetBool("D_holding", isDHolding);
        bool isMoveHolding = isWHolding || isAHolding || isSHolding || isDHolding;
        animator.SetBool("Move_holding", isMoveHolding);
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            bool currentToWalk = animator.GetBool("to_walk");
            animator.SetBool("to_walk", !currentToWalk);
        }
        bool isJumping = Input.GetKey(KeyCode.Space);
        animator.SetBool("isJumping", isJumping);
        bool isAltHolding = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        animator.SetBool("Alt_holding", isAltHolding);
        bool Slide = Input.GetKey(KeyCode.C);
        animator.SetBool("Slide", Slide);
        bool Q_holding = Input.GetKey(KeyCode.Q);
        animator.SetBool("Q_holding", Q_holding);
        bool E_holding = Input.GetKey(KeyCode.E);
        animator.SetBool("E_holding", E_holding);
        bool R_holding = Input.GetKey(KeyCode.R);
        animator.SetBool("R_holding", R_holding);
    }
}

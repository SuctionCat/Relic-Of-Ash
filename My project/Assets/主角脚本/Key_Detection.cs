using UnityEngine;

public class Run_Speed : MonoBehaviour
{
    private Animator animator;
    
    private bool previousIsRunning = false;
    private bool previousMoveHolding = false;
    private bool previousIsJumping = false;
    private bool previousAltHolding = false;
    private bool previousSlide = false;
    private bool previousQHolding = false;
    private bool previousEHolding = false;
    
    private int isRunningHash;
    private int moveHoldingHash;
    private int toWalkHash;
    private int isJumpingHash;
    private int altHoldingHash;
    private int slideHash;
    private int qHoldingHash;
    private int eHoldingHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        moveHoldingHash = Animator.StringToHash("Move_holding");
        toWalkHash = Animator.StringToHash("to_walk");
        isJumpingHash = Animator.StringToHash("isJumping");
        altHoldingHash = Animator.StringToHash("Alt_holding");
        slideHash = Animator.StringToHash("Slide");
        qHoldingHash = Animator.StringToHash("Q_holding");
        eHoldingHash = Animator.StringToHash("E_holding");
    }

    void Update()
    {
        if(GameRoot.GetInstance() != null && GameRoot.GetInstance().IsGamePaused)
            return;
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isRunning != previousIsRunning)
        {
            animator.SetBool(isRunningHash, isRunning);
            previousIsRunning = isRunning;
        }

        bool isWHolding = Input.GetKey(KeyCode.W);
        bool isAHolding = Input.GetKey(KeyCode.A);
        bool isSHolding = Input.GetKey(KeyCode.S);
        bool isDHolding = Input.GetKey(KeyCode.D);
        bool isMoveHolding = isWHolding || isAHolding || isSHolding || isDHolding;
        if (isMoveHolding != previousMoveHolding)
        {
            animator.SetBool(moveHoldingHash, isMoveHolding);
            previousMoveHolding = isMoveHolding;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            bool currentToWalk = animator.GetBool(toWalkHash);
            animator.SetBool(toWalkHash, !currentToWalk);
        }

        bool isJumping = Input.GetKey(KeyCode.Space);
        if (isJumping != previousIsJumping)
        {
            animator.SetBool(isJumpingHash, isJumping);
            previousIsJumping = isJumping;
        }

        bool isAltHolding = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        if (isAltHolding != previousAltHolding)
        {
            animator.SetBool(altHoldingHash, isAltHolding);
            previousAltHolding = isAltHolding;
        }

        bool slide = Input.GetKey(KeyCode.C);
        if (slide != previousSlide)
        {
            animator.SetBool(slideHash, slide);
            previousSlide = slide;
        }

        bool qHolding = Input.GetKey(KeyCode.Q);
        bool canUseQ = StateManager.instance != null && StateManager.instance.IsQReady();
        
        if (qHolding != previousQHolding)
        {
            if (qHolding && canUseQ)
            {
                animator.SetBool(qHoldingHash, true);
            }
            else if (!qHolding)
            {
                animator.SetBool(qHoldingHash, false);
            }
            previousQHolding = qHolding;
        }

        bool eHolding = Input.GetKey(KeyCode.E);
        bool canUseE = StateManager.instance != null && StateManager.instance.IsEReady();
        
        if (eHolding != previousEHolding)
        {
            if (eHolding && canUseE)
            {
                animator.SetBool(eHoldingHash, true);
            }
            else if (!eHolding)
            {
                animator.SetBool(eHoldingHash, false);
            }
            previousEHolding = eHolding;
        }
    }
}

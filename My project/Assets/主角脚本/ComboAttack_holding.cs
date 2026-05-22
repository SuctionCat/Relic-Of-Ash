using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    private Animator animator;
    private bool previousAttack = false;
    private bool previousBlock = false;
    private int attackHash;
    private int blockHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        attackHash = Animator.StringToHash("Attack");
        blockHash = Animator.StringToHash("Block");
    }

    void Update()
    {
        if(GameRoot.GetInstance() != null && GameRoot.GetInstance().IsGamePaused)
            return;
        
        bool isAttack = Input.GetMouseButton(0);
        bool isBlock = Input.GetMouseButton(1);

        if (isAttack != previousAttack)
        {
            animator.SetBool(attackHash, isAttack);
            previousAttack = isAttack;
        }

        if (isBlock != previousBlock)
        {
            animator.SetBool(blockHash, isBlock);
            previousBlock = isBlock;
        }
    }
}
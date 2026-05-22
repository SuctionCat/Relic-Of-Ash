using UnityEngine;


public class ComboByTime : MonoBehaviour
{
    private Animator animator;
    private bool isComboActive = false;
    private bool previousIsComboActive = false;
    private int isComboActiveHash;
    private AnimatorStateInfo currentStateInfo;
    private AnimatorStateInfo previousStateInfo;
    void Start()
    {
        animator = GetComponent<Animator>();
        isComboActiveHash = Animator.StringToHash("isComboActive");
        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        previousStateInfo = currentStateInfo;
    }

    void Update()
    {
        if(GameRoot.GetInstance() != null && GameRoot.GetInstance().IsGamePaused)
            return;
        
        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (Input.GetMouseButtonDown(0))
        {
            isComboActive = true;
        }
        if (currentStateInfo.fullPathHash != previousStateInfo.fullPathHash)
        {
            isComboActive = false;
            previousStateInfo = currentStateInfo;
        }
        if (isComboActive != previousIsComboActive)
        {
            animator.SetBool(isComboActiveHash, isComboActive);
            previousIsComboActive = isComboActive;
        }
    }
}


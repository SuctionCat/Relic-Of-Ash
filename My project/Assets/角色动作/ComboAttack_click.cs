using UnityEngine;


public class ComboByTime : MonoBehaviour
{
    private Animator animator;
    private bool isComboActive = false;
    private AnimatorStateInfo currentStateInfo;
    private AnimatorStateInfo previousStateInfo;
    void Start()
    {
        animator = GetComponent<Animator>();
        // 初始化当前状态信息
        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        previousStateInfo = currentStateInfo;
    }

    void Update()
    {
        // 获取当前动画状态信息
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
        animator.SetBool("isComboActive", isComboActive);
    }
}


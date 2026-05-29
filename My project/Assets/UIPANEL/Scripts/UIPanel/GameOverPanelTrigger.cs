using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanelTrigger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F key detected!");
            LoadScene3();
        }
    }

    private void LoadScene3()
    {
        Debug.Log("F key pressed, loading Scene3...");
        AudioManager.PlayClick();

        // 使用 Unity 原生场景加载方法
        SceneManager.LoadScene("Scene3");
    }
}

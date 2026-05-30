using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanelTrigger : MonoBehaviour
{
    public void OnBossDeath()
    {
        Debug.Log("Boss生命值归零，5秒后加载结束场景...");
        Invoke("LoadEndScene", 5f);
    }

    private void LoadEndScene()
    {
        AudioManager.PlayClick();
        SceneManager.LoadScene("EndScene");
    }
}

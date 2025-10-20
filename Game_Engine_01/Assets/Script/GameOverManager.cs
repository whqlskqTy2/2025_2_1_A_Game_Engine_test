using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Optional UI")]
    public GameObject gameOverPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        // Ȥ�� ������ϰ� ������:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

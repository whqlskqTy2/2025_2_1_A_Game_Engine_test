using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Return To Title")]
    [Tooltip("Ÿ��Ʋ �� �̸� (Build Settings�� ��� �ʼ�)")]
    public string titleSceneName = "TitleScene";

    [Tooltip("���� ���� Ÿ��Ʋ�� ���ư������ ��� �ð� (��)")]
    public float delayBeforeReturn = 3f;

    bool hasTriggered = false; // �ߺ� ȣ�� ����

    // ������ �μ����� �� TreasureHealth.onDestroyed���� ȣ��
    public void OnTreasureDestroyed()
    {
        TriggerGameOver("Treasure destroyed");
    }

    // �÷��̾ �׾��� �� PlayerHealth.onDeath���� ȣ��
    public void OnPlayerDied()
    {
        TriggerGameOver("Player died");
    }

    void TriggerGameOver(string reason)
    {
        if (hasTriggered) return;
        hasTriggered = true;

        Debug.Log("[GameOverManager] GameOver: " + reason + " -> returning to title in " + delayBeforeReturn + "s");

        Invoke(nameof(ReturnToTitle), delayBeforeReturn);
    }

    void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}
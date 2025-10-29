using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Return To Title")]
    [Tooltip("타이틀 씬 이름 (Build Settings에 등록 필수)")]
    public string titleSceneName = "TitleScene";

    [Tooltip("죽은 직후 타이틀로 돌아가기까지 대기 시간 (초)")]
    public float delayBeforeReturn = 3f;

    bool hasTriggered = false; // 중복 호출 방지

    // 보물이 부서졌을 때 TreasureHealth.onDestroyed에서 호출
    public void OnTreasureDestroyed()
    {
        TriggerGameOver("Treasure destroyed");
    }

    // 플레이어가 죽었을 때 PlayerHealth.onDeath에서 호출
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
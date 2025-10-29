using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [Header("스포너 리스트")]
    [Tooltip("감시할 EnemySpawner들을 여기에 등록하세요.")]
    public EnemySpawner[] enemySpawners;

    [Header("클리어 조건")]
    [Tooltip("모든 스포너가 종료된 뒤 엔딩으로 넘어가기까지 대기 시간 (초)")]
    public float delayBeforeWinScene = 3f;

    [Tooltip("엔딩으로 넘어갈 씬 이름 (Build Settings에 등록 필수)")]
    public string endingSceneName = "EndingScene";

    private bool stageCleared = false;

    void Update()
    {
        if (stageCleared) return; // 이미 클리어 체크 끝났으면 종료

        if (AreAllSpawnersFinished())
        {
            stageCleared = true;
            Debug.Log("[WaveManager] 모든 웨이브 클리어! " + delayBeforeWinScene + "초 뒤 엔딩으로 이동");
            Invoke(nameof(GoToEndingScene), delayBeforeWinScene);
        }
    }

    bool AreAllSpawnersFinished()
    {
        if (enemySpawners == null || enemySpawners.Length == 0)
            return false;

        foreach (var spawner in enemySpawners)
        {
            if (spawner == null) continue;

            // EnemySpawner 안에 있는 IsFinished 프로퍼티 사용
            if (!spawner.IsFinished)
                return false;
        }

        return true; // 전부 완료
    }

    void GoToEndingScene()
    {
        SceneManager.LoadScene(endingSceneName);
    }
}
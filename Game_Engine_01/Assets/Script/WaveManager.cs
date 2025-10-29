using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [Header("������ ����Ʈ")]
    [Tooltip("������ EnemySpawner���� ���⿡ ����ϼ���.")]
    public EnemySpawner[] enemySpawners;

    [Header("Ŭ���� ����")]
    [Tooltip("��� �����ʰ� ����� �� �������� �Ѿ����� ��� �ð� (��)")]
    public float delayBeforeWinScene = 3f;

    [Tooltip("�������� �Ѿ �� �̸� (Build Settings�� ��� �ʼ�)")]
    public string endingSceneName = "EndingScene";

    private bool stageCleared = false;

    void Update()
    {
        if (stageCleared) return; // �̹� Ŭ���� üũ �������� ����

        if (AreAllSpawnersFinished())
        {
            stageCleared = true;
            Debug.Log("[WaveManager] ��� ���̺� Ŭ����! " + delayBeforeWinScene + "�� �� �������� �̵�");
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

            // EnemySpawner �ȿ� �ִ� IsFinished ������Ƽ ���
            if (!spawner.IsFinished)
                return false;
        }

        return true; // ���� �Ϸ�
    }

    void GoToEndingScene()
    {
        SceneManager.LoadScene(endingSceneName);
    }
}
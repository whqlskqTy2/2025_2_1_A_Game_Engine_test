using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;     // 생성할 적 프리팹
    public float spawnInterval = 3f;   // 생성 주기 (초)
    public float spawnRange = 5f;      // 생성 범위 (반경)

    private float timer = 0f;          // 시간 측정용

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            // x, z는 랜덤 / y는 고정
            Vector3 spawnPos = new Vector3(
                transform.position.x + Random.Range(-spawnRange, spawnRange),
                transform.position.y,
                transform.position.z + Random.Range(-spawnRange, spawnRange)
            );

            // 적 생성
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            timer = 0f; // 타이머 초기화
        }
    }

    // Scene에서 범위 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 0.1f, spawnRange * 2));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;     // ������ �� ������
    public float spawnInterval = 3f;   // ���� �ֱ� (��)
    public float spawnRange = 5f;      // ���� ���� (�ݰ�)

    private float timer = 0f;          // �ð� ������

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            // x, z�� ���� / y�� ����
            Vector3 spawnPos = new Vector3(
                transform.position.x + Random.Range(-spawnRange, spawnRange),
                transform.position.y,
                transform.position.z + Random.Range(-spawnRange, spawnRange)
            );

            // �� ����
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            timer = 0f; // Ÿ�̸� �ʱ�ȭ
        }
    }

    // Scene���� ���� ǥ��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 0.1f, spawnRange * 2));
    }
}

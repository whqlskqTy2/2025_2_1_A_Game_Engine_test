using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGoal : MonoBehaviour
{
    [Header("기지 HP 설정")]
    public int maxHP = 20;
    public int currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"Base HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            currentHP = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("게임 오버! 기지가 파괴되었습니다.");
        // TODO: 게임 오버 처리
    }

    private void OnTriggerEnter(Collider other)
    {
        // EnemyStatus를 가진 적이 들어오면 처리
        EnemyStatus enemy = other.GetComponent<EnemyStatus>();
        if (enemy != null)
        {
            int dmg = Mathf.Max(1, enemy.damageToBase); // 최소 1
            TakeDamage(dmg);

            Debug.Log($"기지 피격: {enemy.name}에게 {dmg} 피해 받음");

            Destroy(enemy.gameObject);
        }
    }
}
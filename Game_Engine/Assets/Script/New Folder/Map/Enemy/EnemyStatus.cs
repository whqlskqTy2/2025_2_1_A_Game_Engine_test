using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [Header("체력")]
    public int maxHP = 10;
    public int currentHP;

    [Header("기지 도달 시 주는 피해")]
    public int damageToBase = 1;

    private void Awake()
    {
        currentHP = maxHP;
    }

    // 타워 공격 등으로 체력 깎을 때 호출
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: 죽는 연출, 이펙트 등
        Debug.Log($"Enemy {name} 사망");
        Destroy(gameObject);
    }
}
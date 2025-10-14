using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHP = 10;
    private int currentHP;

    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onDeath;

    [Header("Optional FX")]
    public GameObject hitEffect;
    public GameObject deathEffect;

    void Start()
    {
        currentHP = maxHP;
    }

    // 다른 스크립트(적 등)에서 이 함수를 호출해 데미지 적용
    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (currentHP <= 0) return;

        currentHP -= amount;

        if (hitEffect)
            Instantiate(hitEffect, hitPoint, Quaternion.identity);

        onDamaged?.Invoke();

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        onDeath?.Invoke();

        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // 죽었을 때 일단 비활성화 (나중에 리스폰 시스템으로 대체 가능)
        gameObject.SetActive(false);
    }

    public int GetCurrentHP() => currentHP;
}
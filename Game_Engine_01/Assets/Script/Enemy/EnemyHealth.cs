using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHP = 5;
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

        Destroy(gameObject);
    }

    public int GetCurrentHP() => currentHP;
}
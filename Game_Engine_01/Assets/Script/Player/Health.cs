using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [Header("HP")]
    public int maxHP = 10;
    public int currentHP;

    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged; // (current, max)
    public UnityEvent onDeath;

    void Awake()
    {
        currentHP = maxHP;
        onHealthChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (currentHP <= 0) return;
        currentHP = Mathf.Max(0, currentHP - amount);
        onHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
            onDeath?.Invoke();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        onHealthChanged?.Invoke(currentHP, maxHP);
    }
}
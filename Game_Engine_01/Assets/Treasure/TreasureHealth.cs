using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TreasureHealth : MonoBehaviour, IDamageable
{
    [Header("Treasure HP")]
    public int maxHP = 50;
    [SerializeField] private int currentHP;

    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onDestroyed; // 게임 오버 트리거

    [Header("Optional FX")]
    public GameObject hitEffect;
    public GameObject destroyEffect;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (currentHP <= 0) return;

        currentHP = Mathf.Max(0, currentHP - amount);

        if (hitEffect) Instantiate(hitEffect, hitPoint, Quaternion.identity);
        onDamaged?.Invoke();

        if (currentHP <= 0)
        {
            if (destroyEffect) Instantiate(destroyEffect, transform.position, Quaternion.identity);
            onDestroyed?.Invoke(); // 여기에 GameOverManager 연결
        }
    }

    public int CurrentHP => currentHP;
    public float NormalizedHP => maxHP > 0 ? (float)currentHP / maxHP : 0f;
}

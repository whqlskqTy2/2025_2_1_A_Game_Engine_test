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
    public UnityEvent onDestroyed;        // 게임오버 등 외부 반응

    [Header("Optional FX")]
    public GameObject hitEffect;
    public GameObject destroyEffect;

    [Header("Destroy Settings")]
    public bool destroySelfOnZeroHP = true;  // ← 이거 켜면 보물 오브젝트를 지움
    public float destroyDelay = 0f;          // 이펙트 보이게 약간 지연하고 싶으면 사용

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
            onDestroyed?.Invoke(); // UI Game Over, 사운드 등은 여기에 묶기

            if (destroySelfOnZeroHP)
                Destroy(gameObject, destroyDelay); // ← 실제로 오브젝트 제거
        }
    }

    public int CurrentHP => currentHP;
    public float NormalizedHP => maxHP > 0 ? (float)currentHP / maxHP : 0f;
}

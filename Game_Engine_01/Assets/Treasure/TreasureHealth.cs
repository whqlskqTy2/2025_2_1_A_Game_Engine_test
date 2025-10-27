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
    public UnityEvent onHealed;           // 🔸 회복이 일어났을 때 (UI 갱신용, 선택)

    [Header("Optional FX")]
    public GameObject hitEffect;
    public GameObject destroyEffect;

    [Header("Destroy Settings")]
    public bool destroySelfOnZeroHP = true;  // 0이 되면 실제로 오브젝트를 삭제할지
    public float destroyDelay = 0f;

    [Header("Regen Settings")]
    public bool enableRegen = false;         // 🔸 자동 회복 켜기/끄기
    public float regenPerSecond = 2f;        // 🔸 초당 회복량 (float)
    public float regenDelayAfterDamage = 3f; // 🔸 피격 후 몇 초 뒤부터 회복 시작
    public bool canRevive = false;           // 🔸 HP=0 이후도 회복해서 되살아날 수 있게 할지

    private float lastDamagedTime = -999f;   // 마지막으로 맞은 시간

    void Awake()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        HandleRegen();
    }

    // === 데미지 받는 함수 ===
    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (currentHP <= 0 && !canRevive)
            return; // 이미 0이면(부서졌으면) 더 깎지도 않고 무의미

        int oldHP = currentHP;

        currentHP = Mathf.Max(0, currentHP - amount);

        // 맞으면 즉시 회복 타이머 리셋
        lastDamagedTime = Time.time;

        if (hitEffect)
            Instantiate(hitEffect, hitPoint, Quaternion.identity);

        onDamaged?.Invoke();

        // HP가 이번에 처음으로 0 이하로 떨어졌을 때 처리
        if (currentHP <= 0 && oldHP > 0)
        {
            if (destroyEffect)
                Instantiate(destroyEffect, transform.position, Quaternion.identity);

            onDestroyed?.Invoke(); // 게임오버 UI 같은 거 연결

            if (destroySelfOnZeroHP)
            {
                Destroy(gameObject, destroyDelay);
            }
        }
    }

    // === 자동 회복 루틴 ===
    void HandleRegen()
    {
        // 회복 기능이 꺼져 있으면 패스
        if (!enableRegen)
            return;

        // 이미 부서졌고 부활 불가면 회복 안 함
        if (currentHP <= 0 && !canRevive)
            return;

        // 피격 직후 일정 시간은 회복 금지
        if (Time.time < lastDamagedTime + regenDelayAfterDamage)
            return;

        if (currentHP >= maxHP)
            return;

        // 초당 회복: deltaTime을 곱해서 부드럽게 늘린다
        float healFloat = regenPerSecond * Time.deltaTime;

        // currentHP는 int라서 올림 방식을 정해야 한다.
        // 작은 수라도 결국 누적 회복되게 하려면 내부 누적 변수를 따로 두는 방식이 좋지만,
        // 간단하게는 Mathf.CeilToInt로 최소 1씩 회복시키자.
        int healInt = Mathf.CeilToInt(healFloat);

        int oldHP = currentHP;
        currentHP = Mathf.Clamp(currentHP + healInt, 0, maxHP);

        if (currentHP != oldHP)
        {
            onHealed?.Invoke();
        }
    }

    // === 외부에서 수동으로 회복시키고 싶을 때 쓸 수 있는 함수 ===
    public void Heal(int amount)
    {
        if (currentHP <= 0 && !canRevive)
            return;

        int oldHP = currentHP;
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);

        if (currentHP != oldHP)
        {
            onHealed?.Invoke();
        }
    }

    public int CurrentHP => currentHP;
    public float NormalizedHP => maxHP > 0 ? (float)currentHP / maxHP : 0f;
}
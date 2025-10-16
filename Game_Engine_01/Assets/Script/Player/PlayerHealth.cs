using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;           //  UI 사용
using TMPro;                   //  선택: 텍스트를 TMP로 쓸 때

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

    [Header("UI (Optional)")]
    public Slider hpSlider;                 // HUD 슬라이더 (Canvas 아래)
    public Image hpFillImage;              // 슬라이더 Fill 이미지(색 변경용)
    public Gradient hpColorGradient;        // HP 비율에 따른 색상 그라디언트(선택)
    public TextMeshProUGUI hpTextTMP;       // "현재/최대" 표기 (선택)
    public bool showAsPercent = false;      //  78% 형태로 표기

    void Start()
    {
        currentHP = maxHP;
        InitUI();
        RefreshUI();
    }

    // 다른 스크립트(적 등)에서 이 함수를 호출해 데미지 적용
    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (currentHP <= 0) return;

        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);

        if (hitEffect)
            Instantiate(hitEffect, hitPoint, Quaternion.identity);

        onDamaged?.Invoke();
        RefreshUI();

        if (currentHP <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (currentHP <= 0) return;
        currentHP = Mathf.Clamp(currentHP + Mathf.Max(0, amount), 0, maxHP);
        RefreshUI();
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

    // --- UI helpers ---
    void InitUI()
    {
        if (hpSlider)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = maxHP;
            hpSlider.wholeNumbers = true;
        }
    }

    void RefreshUI()
    {
        if (hpSlider)
        {
            hpSlider.value = currentHP;

            // 채우기 색상 그라디언트 적용 (선택)
            if (hpFillImage && hpColorGradient != null)
            {
                float t = maxHP > 0 ? (float)currentHP / maxHP : 0f;
                hpFillImage.color = hpColorGradient.Evaluate(t);
            }
        }

        if (hpTextTMP)
        {
            if (showAsPercent)
            {
                float t = maxHP > 0 ? (float)currentHP / maxHP : 0f;
                hpTextTMP.text = Mathf.RoundToInt(t * 100f) + "%";
            }
            else
            {
                hpTextTMP.text = currentHP + " / " + maxHP;
            }
        }
    }
}
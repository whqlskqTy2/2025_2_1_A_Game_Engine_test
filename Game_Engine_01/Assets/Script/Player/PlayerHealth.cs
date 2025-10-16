using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;           //  UI ���
using TMPro;                   //  ����: �ؽ�Ʈ�� TMP�� �� ��

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
    public Slider hpSlider;                 // HUD �����̴� (Canvas �Ʒ�)
    public Image hpFillImage;              // �����̴� Fill �̹���(�� �����)
    public Gradient hpColorGradient;        // HP ������ ���� ���� �׶���Ʈ(����)
    public TextMeshProUGUI hpTextTMP;       // "����/�ִ�" ǥ�� (����)
    public bool showAsPercent = false;      //  78% ���·� ǥ��

    void Start()
    {
        currentHP = maxHP;
        InitUI();
        RefreshUI();
    }

    // �ٸ� ��ũ��Ʈ(�� ��)���� �� �Լ��� ȣ���� ������ ����
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

        // �׾��� �� �ϴ� ��Ȱ��ȭ (���߿� ������ �ý������� ��ü ����)
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

            // ä��� ���� �׶���Ʈ ���� (����)
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
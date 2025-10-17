using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Refs")]
    public EnemyHealth enemy;     // ����θ� �ڵ����� �θ𿡼� ã��
    public Slider slider;         // ���� �����̽� Canvas �Ʒ� Slider
    public Image fillImage;      // Slider�� Fill �̹���(���� �����, ����)
    public Gradient colorGradient; // HP ������ ���� ��(����)

    [Header("Follow & Billboard")]
    public Transform followTarget;     // ���� ���� �Ӹ�(������ enemy.transform)
    public Vector3 worldOffset = new Vector3(0f, 2.0f, 0f); // �Ӹ� �� ������
    public Camera targetCamera;        // ����θ� Camera.main
    public bool billboard = true;      // ī�޶� ���� ȸ��
    public bool keepUpright = true;    // ȸ�� �� X/Z ���� ����

    [Header("Visibility")]
    public bool hideWhenFull = true;   // HP�� �� �� ������ �����
    public float minVisibleRatio = 0.001f; // 0�� ������ ���� ���� �Ӱ谪

    void Awake()
    {
        if (!enemy) enemy = GetComponentInParent<EnemyHealth>();
        if (!followTarget) followTarget = enemy ? enemy.transform : transform.parent;
        if (!targetCamera) targetCamera = Camera.main;

        if (enemy == null || slider == null)
        {
            Debug.LogWarning("[EnemyHealthBar] enemy/slider ������ �ʿ��մϴ�.", this);
            enabled = false;
            return;
        }
    }

    void OnEnable()
    {
        // �ʱ�ȭ
        InitSlider();
        Refresh();
    }

    void Update()
    {
        // ��ġ ���󰡱�
        if (followTarget)
        {
            transform.position = followTarget.position + worldOffset;
        }

        // ī�޶� ���ϵ���(������)
        if (billboard && targetCamera)
        {
            Vector3 toCam = targetCamera.transform.position - transform.position;
            if (keepUpright) toCam.y = 0f;
            if (toCam.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(-toCam.normalized, Vector3.up);
        }
    }

    // �ܺ�(�ִϸ��̼� �̺�Ʈ ��)���� ȣ���� ���� �ְ� public
    public void Refresh()
    {
        if (enemy == null) return;

        int cur = Mathf.Max(0, enemy.GetCurrentHP());
        int max = Mathf.Max(1, enemy.maxHP);

        slider.minValue = 0;
        slider.maxValue = max;
        slider.value = cur;

        float t = (float)cur / max;

        // ���� �׶���Ʈ ����(����)
        if (fillImage && colorGradient != null)
        {
            fillImage.color = colorGradient.Evaluate(t);
        }

        // Ǯ HP�� ����� �ɼ�
        if (hideWhenFull)
        {
            bool visible = t < 1f - minVisibleRatio;
            if (slider.gameObject.activeSelf != visible)
                slider.gameObject.SetActive(visible);
        }
    }

    void InitSlider()
    {
        slider.minValue = 0;
        slider.maxValue = Mathf.Max(1, enemy.maxHP);
        slider.wholeNumbers = true;
    }

    // EnemyHealth�� UnityEvent(onDamaged/onDeath)�� �����ϸ� �ڵ� ���ŵ�
    // �ν����� �̺�Ʈ�� EnemyHealthBar.Refresh()�� ����ϼ���.
}
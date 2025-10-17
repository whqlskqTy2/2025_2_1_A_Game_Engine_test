using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Refs")]
    public EnemyHealth enemy;     // 비워두면 자동으로 부모에서 찾음
    public Slider slider;         // 월드 스페이스 Canvas 아래 Slider
    public Image fillImage;      // Slider의 Fill 이미지(색상 변경용, 선택)
    public Gradient colorGradient; // HP 비율에 따른 색(선택)

    [Header("Follow & Billboard")]
    public Transform followTarget;     // 보통 적의 머리(없으면 enemy.transform)
    public Vector3 worldOffset = new Vector3(0f, 2.0f, 0f); // 머리 위 오프셋
    public Camera targetCamera;        // 비워두면 Camera.main
    public bool billboard = true;      // 카메라를 향해 회전
    public bool keepUpright = true;    // 회전 시 X/Z 기울기 방지

    [Header("Visibility")]
    public bool hideWhenFull = true;   // HP가 꽉 차 있으면 숨기기
    public float minVisibleRatio = 0.001f; // 0에 가까우면 숨김 방지 임계값

    void Awake()
    {
        if (!enemy) enemy = GetComponentInParent<EnemyHealth>();
        if (!followTarget) followTarget = enemy ? enemy.transform : transform.parent;
        if (!targetCamera) targetCamera = Camera.main;

        if (enemy == null || slider == null)
        {
            Debug.LogWarning("[EnemyHealthBar] enemy/slider 참조가 필요합니다.", this);
            enabled = false;
            return;
        }
    }

    void OnEnable()
    {
        // 초기화
        InitSlider();
        Refresh();
    }

    void Update()
    {
        // 위치 따라가기
        if (followTarget)
        {
            transform.position = followTarget.position + worldOffset;
        }

        // 카메라를 향하도록(빌보드)
        if (billboard && targetCamera)
        {
            Vector3 toCam = targetCamera.transform.position - transform.position;
            if (keepUpright) toCam.y = 0f;
            if (toCam.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(-toCam.normalized, Vector3.up);
        }
    }

    // 외부(애니메이션 이벤트 등)에서 호출할 수도 있게 public
    public void Refresh()
    {
        if (enemy == null) return;

        int cur = Mathf.Max(0, enemy.GetCurrentHP());
        int max = Mathf.Max(1, enemy.maxHP);

        slider.minValue = 0;
        slider.maxValue = max;
        slider.value = cur;

        float t = (float)cur / max;

        // 색상 그라디언트 적용(선택)
        if (fillImage && colorGradient != null)
        {
            fillImage.color = colorGradient.Evaluate(t);
        }

        // 풀 HP면 숨기기 옵션
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

    // EnemyHealth의 UnityEvent(onDamaged/onDeath)와 연결하면 자동 갱신됨
    // 인스펙터 이벤트에 EnemyHealthBar.Refresh()를 등록하세요.
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class TreasureProjectile : MonoBehaviour
{
    public enum OwnerType { Neutral, Player, Enemy }

    [Header("Motion")]
    public float speed = 18f;
    public float lifetime = 4f;

    [Header("Damage")]
    public int damage = 1;
    public OwnerType owner = OwnerType.Neutral;

    [Header("Hit Filter (optional)")]
    [Tooltip("비워두면 모든 IDamageable에 데미지. 값이 있으면 해당 태그를 가진 오브젝트(또는 그 부모)에게만 데미지.")]
    public string onlyDamageTag = "";
    [Tooltip("맞출 레이어만 통과시키기(비워두면 전체).")]
    public LayerMask hitLayers = ~0;

    [Header("FX (optional)")]
    public GameObject hitEffect;

    Rigidbody rb;

    // 내부 이동 방향(물리 안 쓸 때)
    Vector3 moveDir = Vector3.zero;

    // ==== 선택형: 발사 시 세팅 ====
    // - direction: 진행 방향
    // - speedOverride < 0 면 기존 speed 유지
    // - ignoreColliders: 발사자 콜라이더(들) 전달 시, 초기에 충돌 무시
    public void Init(Vector3 direction, float speedOverride = -1f, int damageOverride = -1, Collider[] ignoreColliders = null)
    {
        if (direction.sqrMagnitude > 0.0001f)
            moveDir = direction.normalized;

        if (speedOverride >= 0f) speed = speedOverride;
        if (damageOverride >= 0) damage = damageOverride;

        // 회전 정렬
        if (moveDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveDir);

        // 발사자와 충돌 무시(있을 때만)
        if (ignoreColliders != null)
        {
            var myCol = GetComponent<Collider>();
            foreach (var col in ignoreColliders)
                if (col && myCol) Physics.IgnoreCollision(myCol, col, true);
        }

        // 물리 이동 사용 시 초기 속도 부여
        if (rb && !rb.isKinematic)
        {
            rb.useGravity = false;
            rb.velocity = moveDir * speed;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // 수명 타이머
        CancelInvoke(nameof(Despawn));
        Invoke(nameof(Despawn), lifetime);

        // Fire/Init를 호출 안 해도 기본적으로 forward로 진행
        if (moveDir == Vector3.zero)
        {
            var fwd = transform.forward;
            moveDir = fwd.sqrMagnitude > 0.0001f ? fwd.normalized : Vector3.forward;
        }

        if (rb && !rb.isKinematic)
        {
            rb.useGravity = false;
            rb.velocity = moveDir * speed;
        }
    }

    void Start()
    {
        // Rigidbody 안 쓰는 경우엔 Update에서 이동
        // (rb && !isKinematic)면 여기서 따로 할 일 없음
    }

    void Update()
    {
        if (!rb || rb.isKinematic)
            transform.position += moveDir * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // 레이어 필터
        if (((1 << other.gameObject.layer) & hitLayers) == 0) return;

        // 태그 필터(옵션)
        if (!string.IsNullOrEmpty(onlyDamageTag))
        {
            if (!(other.CompareTag(onlyDamageTag) ||
                  (other.transform.root != null && other.transform.root.CompareTag(onlyDamageTag)) ||
                  (other.transform.parent != null && other.transform.parent.CompareTag(onlyDamageTag))))
            {
                return;
            }
        }

        // 팀킬 방지(원하면 사용) — 필요 시 주석 해제
        // if (owner == OwnerType.Enemy && other.GetComponentInParent<EnemyHealth>()) return;
        // if (owner == OwnerType.Player && other.GetComponentInParent<PlayerHealth>()) return;

        // 데미지 대상 탐색
        var dmg = other.GetComponentInParent<IDamageable>() ?? other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage, transform.position);
            if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);
            Despawn();
        }
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
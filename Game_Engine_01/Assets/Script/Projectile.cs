using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public enum OwnerType { Player, Enemy }

    [Header("Settings")]
    public OwnerType owner = OwnerType.Player;
    public float speed = 20f;
    public int damage = 1;
    public float lifetime = 3f;

    [Header("FX (optional)")]
    public GameObject hitEffect;

    private Rigidbody rb;
    private Vector3 moveDir = Vector3.zero; // 물리 없을 때도 이동하도록 저장

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // 없어도 동작하게 설계
        // Collider는 IsTrigger 권장
    }

    void OnEnable()
    {
        Invoke(nameof(Despawn), lifetime);
    }

    // 발사 시 반드시 호출
    public void Fire(Vector3 direction)
    {
        direction = direction.normalized;
        if (direction == Vector3.zero)
        {
            // 방향이 0이면 앞방향으로라도 보정
            direction = transform.forward.sqrMagnitude > 0.001f ? transform.forward.normalized : Vector3.forward;
        }

        moveDir = direction;
        transform.rotation = Quaternion.LookRotation(direction);

        if (rb != null && !rb.isKinematic)
        {
            rb.velocity = direction * speed;
        }
    }

    void Update()
    {
        // Rigidbody가 없거나 isKinematic인 경우에도 전진
        if (rb == null || rb.isKinematic)
        {
            transform.position += moveDir * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 아군/적군 필터
        if (owner == OwnerType.Player)
        {
            var eh = other.GetComponent<EnemyHealth>() ?? other.GetComponentInParent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage, transform.position);
                SpawnHitFX();
                Despawn();
            }
        }
        else
        {
            var ph = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage, transform.position);
                SpawnHitFX();
                Despawn();
            }
        }
    }

    void SpawnHitFX()
    {
        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
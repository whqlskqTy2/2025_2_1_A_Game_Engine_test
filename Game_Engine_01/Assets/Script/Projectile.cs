using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum OwnerType { Player, Enemy }

    [Header("Projectile")]
    public OwnerType owner = OwnerType.Player;
    public float speed = 20f;
    public int damage = 1;
    public float lifetime = 3f;

    [Header("FX (optional)")]
    public GameObject hitEffect;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        Invoke(nameof(Despawn), lifetime);
    }

    public void Fire(Vector3 direction)
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        rb.velocity = direction.normalized * speed;
        transform.forward = direction.normalized;
    }

    void OnTriggerEnter(Collider other)
    {
        // 자기 진영 무시
        if (owner == OwnerType.Player && other.GetComponent<EnemyHealth>() == null && other.GetComponentInParent<EnemyHealth>() == null)
            return;
        if (owner == OwnerType.Enemy && other.GetComponent<PlayerHealth>() == null && other.GetComponentInParent<PlayerHealth>() == null)
            return;

        // 실제 데미지 처리
        Vector3 hitPoint = transform.position;

        if (owner == OwnerType.Player)
        {
            var eh = other.GetComponent<EnemyHealth>() ?? other.GetComponentInParent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage, hitPoint);
                SpawnHitFX();
                Despawn();
            }
        }
        else // Enemy 탄환 → Player만
        {
            var ph = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage, hitPoint);
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
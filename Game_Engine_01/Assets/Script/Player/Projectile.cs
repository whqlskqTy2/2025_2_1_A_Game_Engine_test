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
    private Vector3 moveDir = Vector3.zero; // ���� ���� ���� �̵��ϵ��� ����

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // ��� �����ϰ� ����
        // Collider�� IsTrigger ����
    }

    void OnEnable()
    {
        Invoke(nameof(Despawn), lifetime);
    }

    // �߻� �� �ݵ�� ȣ��
    public void Fire(Vector3 direction)
    {
        direction = direction.normalized;
        if (direction == Vector3.zero)
        {
            // ������ 0�̸� �չ������ζ� ����
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
        // Rigidbody�� ���ų� isKinematic�� ��쿡�� ����
        if (rb == null || rb.isKinematic)
        {
            transform.position += moveDir * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // �Ʊ�/���� ����
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
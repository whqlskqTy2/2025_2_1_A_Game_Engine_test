using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class TreasureProjectile : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 18f;
    public float lifetime = 4f;

    [Header("Damage")]
    public int damage = 1;

    Rigidbody rb; // 있어도 되고 없어도 됨

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);

        // Rigidbody 쓰면 물리로, 없으면 Transform로
        if (rb && !rb.isKinematic)
        {
            rb.useGravity = false;
            rb.velocity = transform.forward * speed;
        }
    }

    void Update()
    {
        // Rigidbody 안 쓰는 경우에만 직접 이동
        if (!rb || rb.isKinematic)
            transform.position += transform.forward * speed * Time.deltaTime;
    }

    // 필요시 충돌 처리 (IDamageable 호출)
    void OnTriggerEnter(Collider other)
    {
        var dmg = other.GetComponentInParent<IDamageable>() ?? other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
    }
}
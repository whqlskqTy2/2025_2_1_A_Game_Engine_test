using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SimpleProjectile : MonoBehaviour
{
    public float lifetime = 2f;
    public bool destroyOnHit = true;

    private int damage = 1;
    private float speed = 20f;
    private string ownerTag = "Player";
    private float spawnTime;

    public void Init(int damage, float speed, string ownerTag)
    {
        this.damage = damage;
        this.speed = speed;
        this.ownerTag = ownerTag;
    }

    void OnEnable()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        if (Time.time - spawnTime > lifetime)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // 아군 피격 방지
        if (other.CompareTag(ownerTag)) return;

        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            Vector3 hp = other.ClosestPoint(transform.position);
            dmg.TakeDamage(damage, hp, -transform.forward);
        }

        if (destroyOnHit) Destroy(gameObject);
    }
}
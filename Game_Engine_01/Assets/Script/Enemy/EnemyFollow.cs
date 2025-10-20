using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // ← 비워두면 태그로 자동 탐색
    public string targetTag = "Treasure"; // 보물에 이 태그 붙이기

    [Header("Follow")]
    public float followRange = 15f;
    public float stopDistance = 2.5f;
    public float moveSpeed = 3f;
    public float gravity = -18f;
    public bool rotateOnlyY = true;

    CharacterController cc;
    Vector3 velocity;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (target == null)
        {
            var t = GameObject.FindGameObjectWithTag(targetTag);
            if (t) target = t.transform;
        }
    }

    void Update()
    {
        if (!target) return;

        // 간이 중력
        if (cc.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= followRange && dist > stopDistance)
        {
            Vector3 to = (target.position - transform.position);
            if (rotateOnlyY) to.y = 0;

            // 회전
            if (to.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(to),
                    Time.deltaTime * 8f
                );

            // 이동
            Vector3 move = to.normalized * moveSpeed;
            move.y = velocity.y;
            cc.Move(move * Time.deltaTime);
        }
        else
        {
            // 제자리에서도 중력 적용
            cc.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
        }
    }
}

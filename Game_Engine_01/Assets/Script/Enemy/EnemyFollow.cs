using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;             // 따라갈 대상(플레이어)
    public float followRange = 15f;      // 추적 시작 거리
    public float stopDistance = 2.5f;    // 너무 가까워지면 멈춤

    [Header("Movement")]
    public float moveSpeed = 3f;         // 이동 속도
    public float gravity = -18f;         // 중력

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (player == null)
        {
            // 자동으로 Player 태그를 가진 오브젝트 탐색
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= followRange && distance > stopDistance)
        {
            // 플레이어 방향으로 이동
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f; // 평면 이동

            controller.Move(dir * moveSpeed * Time.deltaTime);

            // 플레이어 쪽으로 회전
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
        }

        // 중력 적용
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // 디버그용 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;             // ���� ���(�÷��̾�)
    public float followRange = 15f;      // ���� ���� �Ÿ�
    public float stopDistance = 2.5f;    // �ʹ� ��������� ����

    [Header("Movement")]
    public float moveSpeed = 3f;         // �̵� �ӵ�
    public float gravity = -18f;         // �߷�

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (player == null)
        {
            // �ڵ����� Player �±׸� ���� ������Ʈ Ž��
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
            // �÷��̾� �������� �̵�
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f; // ��� �̵�

            controller.Move(dir * moveSpeed * Time.deltaTime);

            // �÷��̾� ������ ȸ��
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
        }

        // �߷� ����
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ����׿� �ð�ȭ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
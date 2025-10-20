using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // �� ����θ� �±׷� �ڵ� Ž��
    public string targetTag = "Treasure"; // ������ �� �±� ���̱�

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

        // ���� �߷�
        if (cc.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= followRange && dist > stopDistance)
        {
            Vector3 to = (target.position - transform.position);
            if (rotateOnlyY) to.y = 0;

            // ȸ��
            if (to.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(to),
                    Time.deltaTime * 8f
                );

            // �̵�
            Vector3 move = to.normalized * moveSpeed;
            move.y = velocity.y;
            cc.Move(move * Time.deltaTime);
        }
        else
        {
            // ���ڸ������� �߷� ����
            cc.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
        }
    }
}

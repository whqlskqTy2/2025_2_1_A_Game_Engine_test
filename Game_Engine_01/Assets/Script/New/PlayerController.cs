using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;         // �⺻ �̵� �ӵ�
    public float runMultiplier = 1.5f;   // �޸��� ���(LeftShift)
    public float rotationSmooth = 12f;   // ȸ�� �ε巴��

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.6f;      // ���� ����(���� ����)
    public float gravity = -20f;         // �߷� ���ӵ�
    public float groundedStick = -2f;    // ���� �����(���鿡�� Ƣ�� �� ����)

    [Header("Camera")]
    public Transform cameraPivot;         // ī�޶�(�Ǵ� ī�޶� Ȧ��) Transform. ������ ���� ���� �̵�.

    private CharacterController controller;
    private Vector3 velocity;             // y��(�߷�/����) ����
    private bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ====== Ground Check ======
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
        {
            // ���鿡�� ���Ÿ� ����
            velocity.y = groundedStick;
        }

        // ====== �Է�(ī�޶� ���� WASD) ======
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");   // W/S
        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        // �̵� ������ ī�޶� �������� ��ȯ
        Vector3 moveDir = Vector3.zero;
        if (inputDir.sqrMagnitude > 0.0001f)
        {
            if (cameraPivot)
            {
                Vector3 camForward = cameraPivot.forward; camForward.y = 0f; camForward.Normalize();
                Vector3 camRight = cameraPivot.right; camRight.y = 0f; camRight.Normalize();
                moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
            }
            else
            {
                moveDir = inputDir; // ī�޶� �ǹ� ������ ���� ����
            }

            // �ٶ󺸴� ���� ȸ��
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSmooth * Time.deltaTime);
        }

        // �޸���(LeftShift)
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? runMultiplier : 1f);

        // ���� �̵�(����)
        Vector3 horizontal = moveDir * speed;

        // ====== ���� ======
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // v^2 = 2gh -> v = sqrt(2 * jumpHeight * -gravity)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // ====== �߷� ======
        velocity.y += gravity * Time.deltaTime;

        // ====== ���� �̵� ======
        Vector3 finalMove = horizontal + new Vector3(0f, velocity.y, 0f);
        controller.Move(finalMove * Time.deltaTime);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;         // 기본 이동 속도
    public float runMultiplier = 1.5f;   // 달리기 배수(LeftShift)
    public float rotationSmooth = 12f;   // 회전 부드럽게

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.6f;      // 점프 높이(미터 느낌)
    public float gravity = -20f;         // 중력 가속도
    public float groundedStick = -2f;    // 지면 붙잡기(경사면에서 튀는 것 방지)

    [Header("Camera")]
    public Transform cameraPivot;         // 카메라(또는 카메라 홀더) Transform. 없으면 월드 기준 이동.

    private CharacterController controller;
    private Vector3 velocity;             // y축(중력/점프) 보관
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
            // 경사면에서 들썩거림 방지
            velocity.y = groundedStick;
        }

        // ====== 입력(카메라 기준 WASD) ======
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");   // W/S
        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        // 이동 방향을 카메라 기준으로 변환
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
                moveDir = inputDir; // 카메라 피벗 없으면 월드 기준
            }

            // 바라보는 방향 회전
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSmooth * Time.deltaTime);
        }

        // 달리기(LeftShift)
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? runMultiplier : 1f);

        // 실제 이동(수평)
        Vector3 horizontal = moveDir * speed;

        // ====== 점프 ======
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // v^2 = 2gh -> v = sqrt(2 * jumpHeight * -gravity)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // ====== 중력 ======
        velocity.y += gravity * Time.deltaTime;

        // ====== 최종 이동 ======
        Vector3 finalMove = horizontal + new Vector3(0f, velocity.y, 0f);
        controller.Move(finalMove * Time.deltaTime);
    }
}
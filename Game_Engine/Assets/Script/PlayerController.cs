using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 10f;
    public float jumpPower = 5f;
    public float gravity = -9.81f;

    public CharacterController controller;
    public float rotationSpeed = 10f;

    private CinemachinePOV pov;
    public CinemachineVirtualCamera virtualCam;

    public Slider hpSlider;


    private Vector3 velocity;
    private bool isGrounded;

    public int maxHP = 100;   // 최대 체력
    private int currentHP;    // 현재 체력

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        hpSlider.value = (float)currentHP / maxHP;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }



    // ⬇️ FreeLook 모드 상태를 알기 위해 스위처 참조
    [SerializeField] private CinemacineSwither camSwitcher;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (virtualCam != null)
            pov = virtualCam.GetCinemachineComponent<CinemachinePOV>();

        hpSlider.value = 1f;
        currentHP = maxHP;


        // 인스펙터에서 안 꽂아뒀으면 자동 검색
        if (camSwitcher == null)
            camSwitcher = FindObjectOfType<CinemacineSwither>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            pov.m_HorizontalAxis.Value = transform.eulerAngles.y;
            pov.m_VerticalAxis.Value = 0f;
        }

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f; // 지면에 살짝 붙임

        //  FreeLook 모드일 때: 이동/회전/점프 전부 차단
        if (camSwitcher != null && camSwitcher.usingFreeLook)
        {
            // 수평 속도 제거, 회전/입력 무시
            velocity.x = 0f;
            velocity.z = 0f;

            // 공중이면 중력만 적용해서 자연스러운 낙하 유지
            if (!isGrounded)
                velocity.y += gravity * Time.deltaTime;

            // 수직 이동만 반영
            controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);
            return;
        }


        //  이하 일반 조작
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 카메라 기준 이동 벡터
        Vector3 camForward = virtualCam.transform.forward; camForward.y = 0f; camForward.Normalize();
        Vector3 camRight = virtualCam.transform.right; camRight.y = 0f; camRight.Normalize();
        Vector3 move = (camForward * z + camRight * x).normalized;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 카메라 yaw 기준으로 플레이어 회전
        if (pov != null)
        {
            float cameraYaw = pov.m_HorizontalAxis.Value;
            Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // 점프
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            velocity.y = jumpPower;

        // 중력
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
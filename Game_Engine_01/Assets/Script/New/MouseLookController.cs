using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookController : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensitivity = 240f;     // 마우스 감도(도/초)
    public float smooth = 12f;           // 회전 보간(부드럽게)

    [Header("Pitch Clamp")]
    public float minPitch = -60f;        // 아래로
    public float maxPitch = 80f;        // 위로

    [Header("Options")]
    public bool invertY = false;         // 상하 반전

    [Header("Player")]
    public Transform playerBody;         // 좌우(Yaw)는 플레이어가 회전

    [Header("Aim (RMB)")]
    public Camera cam;                   // 메인 카메라
    public float normalFOV = 60f;
    public float aimFOV = 42f;
    public float fovLerp = 10f;          // FOV 보간 속도

    float targetYaw;     // 수평 누적
    float targetPitch;   // 수직 누적

    void Start()
    {
        // 초기 각도 동기화
        Vector3 e = playerBody ? playerBody.eulerAngles : Vector3.zero;
        targetYaw = e.y;
        targetPitch = transform.localEulerAngles.x;

        // 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!cam) cam = Camera.main;
        if (cam) cam.fieldOfView = normalFOV;
    }

    void Update()
    {
        // ===== 입력 =====
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        if (invertY) mouseY = -mouseY;

        targetYaw += mouseX;
        targetPitch -= mouseY; // 위로 올리면 pitch 증가가 아닌 감소 방향으로 잡는 게 일반적
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        // ===== 회전 적용(부드럽게 보간) =====
        if (playerBody)
        {
            Quaternion currentYaw = playerBody.rotation;
            Quaternion desiredYaw = Quaternion.Euler(0f, targetYaw, 0f);
            playerBody.rotation = Quaternion.Slerp(currentYaw, desiredYaw, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        }

        Quaternion currentPitch = transform.localRotation;
        Quaternion desiredPitch = Quaternion.Euler(targetPitch, 0f, 0f);
        transform.localRotation = Quaternion.Slerp(currentPitch, desiredPitch, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        // ===== 조준(우클릭) FOV =====
        if (cam)
        {
            float targetFov = Input.GetMouseButton(1) ? aimFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, 1f - Mathf.Exp(-fovLerp * Time.deltaTime));
        }

        // ===== 커서 해제/재잠금 =====
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
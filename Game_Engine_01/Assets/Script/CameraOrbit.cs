using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                 // 플레이어 Transform

    [Header("Orbit")]
    public bool holdRightMouseToOrbit = true;
    public float sensX = 120f;               // 마우스 X 감도(수평)
    public float sensY = 90f;                // 마우스 Y 감도(수직)
    public float minPitch = 10f;             // 아래로 너무 안 보이게
    public float maxPitch = 70f;             // 탑뷰 각도 한계
    public float yaw = 0f;                   // 수평 각도(초기값)
    public float pitch = 35f;                // 수직 각도(초기값)

    [Header("Zoom")]
    public float distance = 10f;             // 기본 거리
    public float minDistance = 4f;
    public float maxDistance = 18f;
    public float zoomSpeed = 6f;

    [Header("Follow Smoothing")]
    public float followLerp = 12f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float collideRadius = 0.2f;       // 카메라 충돌 보정(선택)

    [Header("Cursor")]
    public bool lockCursorWhenOrbit = false;

    void LateUpdate()
    {
        if (!target) return;

        // --- 입력 읽기 (신/구 시스템 모두 지원) ---
        float mx = 0f, my = 0f, scroll = 0f;
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            var delta = mouse.delta.ReadValue();
            mx = delta.x;
            my = delta.y;
            scroll = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y * 0.1f;
        }
#else
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");
        scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;
#endif

        bool orbiting =
            !holdRightMouseToOrbit
            ||
#if ENABLE_INPUT_SYSTEM
            (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.rightButton.isPressed)
#else
            Input.GetMouseButton(1)
#endif
        ;

        // --- 회전 ---
        if (orbiting)
        {
            yaw += mx * sensX * Time.deltaTime;
            pitch -= my * sensY * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            if (lockCursorWhenOrbit)
                Cursor.lockState = CursorLockMode.Locked;
        }
        else if (lockCursorWhenOrbit)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // --- 줌 ---
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // --- 원하는 위치/회전 계산 ---
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position - rot * Vector3.forward * distance;

        // --- 충돌 보정(옵션) ---
        if (collisionMask.value != 0)
        {
            Vector3 dir = (desiredPos - target.position).normalized;
            float dist = distance;
            if (Physics.SphereCast(target.position, collideRadius, dir, out RaycastHit hit, distance, collisionMask))
            {
                dist = Mathf.Max(0.6f, hit.distance - 0.1f);
                desiredPos = target.position + dir * dist;
            }
        }

        // --- 부드럽게 따라가기 ---
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }
}
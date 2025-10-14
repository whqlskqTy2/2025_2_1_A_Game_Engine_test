using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_ToggleView : MonoBehaviour
{
    [Header("Target")]
    public Transform target;           // 플레이어

    [Header("Orbit Settings")]
    public float sensX = 120f;
    public float sensY = 90f;
    public float minPitch = 10f;
    public float maxPitch = 70f;
    public float yaw = 0f;
    public float pitch = 35f;
    public float distance = 10f;
    public float minDistance = 4f;
    public float maxDistance = 18f;
    public float zoomSpeed = 6f;

    [Header("Follow Smooth")]
    public float followLerp = 12f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float collideRadius = 0.2f;

    [Header("Free Look Mode")]
    public bool isFreeLook = false;       // 현재 자유 시점 모드 여부
    public KeyCode toggleKey = KeyCode.BackQuote;  // ` 키로 전환
    public float freeLookSens = 120f;     // 자유 시점 회전 감도

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // ===== 시점 전환 =====
        if (Input.GetKeyDown(toggleKey))
        {
            isFreeLook = !isFreeLook;
        }

        // ===== 마우스 입력 =====
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        float scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;

        if (!isFreeLook)
        {
            // 기본 TPS 모드: 카메라와 캐릭터 같이 회전
            yaw += mx * sensX * Time.deltaTime;
            pitch -= my * sensY * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        else
        {
            // 자유 시점 모드: 카메라만 회전 (플레이어 고정)
            yaw += mx * freeLookSens * Time.deltaTime;
            pitch -= my * freeLookSens * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // 줌
        if (Mathf.Abs(scroll) > 0.001f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // 회전 결과 적용
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position - rot * Vector3.forward * distance;

        // 충돌 보정
        if (collisionMask.value != 0)
        {
            Vector3 dir = (desiredPos - target.position).normalized;
            if (Physics.SphereCast(target.position, collideRadius, dir, out RaycastHit hit, distance, collisionMask))
                desiredPos = target.position + dir * (hit.distance - 0.3f);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }

    // 카메라가 현재 바라보는 방향을 반환 (플레이어가 이걸로 총알을 쏨)
    public Vector3 GetAimDirection()
    {
        return transform.forward;
    }
}
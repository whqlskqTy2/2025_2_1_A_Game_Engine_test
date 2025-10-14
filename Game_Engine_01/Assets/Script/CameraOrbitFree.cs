using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbitFree : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // 따라갈 플레이어

    [Header("Orbit Settings")]
    public float sensX = 120f;            // 마우스 X 회전 감도
    public float sensY = 90f;             // 마우스 Y 회전 감도
    public float minPitch = 10f;          // 아래쪽 제한
    public float maxPitch = 70f;          // 위쪽 제한
    public float yaw = 0f;                // 초기 수평 각도
    public float pitch = 35f;             // 초기 수직 각도

    [Header("Zoom Settings")]
    public float distance = 10f;          // 기본 거리
    public float minDistance = 4f;
    public float maxDistance = 18f;
    public float zoomSpeed = 6f;

    [Header("Follow Smoothing")]
    public float followLerp = 12f;

    [Header("Collision (Optional)")]
    public LayerMask collisionMask;
    public float collideRadius = 0.2f;

    void Start()
    {
        // 마우스 고정 및 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // --- 마우스 입력 ---
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        float scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;

        // 회전 업데이트
        yaw += mx * sensX * Time.deltaTime;
        pitch -= my * sensY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 줌 업데이트
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // 카메라 위치 계산
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position - rot * Vector3.forward * distance;

        // 카메라 충돌 보정
        if (collisionMask.value != 0)
        {
            Vector3 dir = (desiredPos - target.position).normalized;
            if (Physics.SphereCast(target.position, collideRadius, dir, out RaycastHit hit, distance, collisionMask))
            {
                desiredPos = target.position + dir * (hit.distance - 0.3f);
            }
        }

        // 부드러운 이동
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }
}
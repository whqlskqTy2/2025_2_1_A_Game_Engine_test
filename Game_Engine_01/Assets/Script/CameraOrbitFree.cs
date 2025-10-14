using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbitFree : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // ���� �÷��̾�

    [Header("Orbit Settings")]
    public float sensX = 120f;            // ���콺 X ȸ�� ����
    public float sensY = 90f;             // ���콺 Y ȸ�� ����
    public float minPitch = 10f;          // �Ʒ��� ����
    public float maxPitch = 70f;          // ���� ����
    public float yaw = 0f;                // �ʱ� ���� ����
    public float pitch = 35f;             // �ʱ� ���� ����

    [Header("Zoom Settings")]
    public float distance = 10f;          // �⺻ �Ÿ�
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
        // ���콺 ���� �� ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // --- ���콺 �Է� ---
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        float scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;

        // ȸ�� ������Ʈ
        yaw += mx * sensX * Time.deltaTime;
        pitch -= my * sensY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // �� ������Ʈ
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // ī�޶� ��ġ ���
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position - rot * Vector3.forward * distance;

        // ī�޶� �浹 ����
        if (collisionMask.value != 0)
        {
            Vector3 dir = (desiredPos - target.position).normalized;
            if (Physics.SphereCast(target.position, collideRadius, dir, out RaycastHit hit, distance, collisionMask))
            {
                desiredPos = target.position + dir * (hit.distance - 0.3f);
            }
        }

        // �ε巯�� �̵�
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }
}
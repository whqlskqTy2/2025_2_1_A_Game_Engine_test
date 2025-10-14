using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_ToggleView : MonoBehaviour
{
    [Header("Target")]
    public Transform target;           // �÷��̾�

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
    public bool isFreeLook = false;       // ���� ���� ���� ��� ����
    public KeyCode toggleKey = KeyCode.BackQuote;  // ` Ű�� ��ȯ
    public float freeLookSens = 120f;     // ���� ���� ȸ�� ����

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // ===== ���� ��ȯ =====
        if (Input.GetKeyDown(toggleKey))
        {
            isFreeLook = !isFreeLook;
        }

        // ===== ���콺 �Է� =====
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        float scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;

        if (!isFreeLook)
        {
            // �⺻ TPS ���: ī�޶�� ĳ���� ���� ȸ��
            yaw += mx * sensX * Time.deltaTime;
            pitch -= my * sensY * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        else
        {
            // ���� ���� ���: ī�޶� ȸ�� (�÷��̾� ����)
            yaw += mx * freeLookSens * Time.deltaTime;
            pitch -= my * freeLookSens * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // ��
        if (Mathf.Abs(scroll) > 0.001f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // ȸ�� ��� ����
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position - rot * Vector3.forward * distance;

        // �浹 ����
        if (collisionMask.value != 0)
        {
            Vector3 dir = (desiredPos - target.position).normalized;
            if (Physics.SphereCast(target.position, collideRadius, dir, out RaycastHit hit, distance, collisionMask))
                desiredPos = target.position + dir * (hit.distance - 0.3f);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }

    // ī�޶� ���� �ٶ󺸴� ������ ��ȯ (�÷��̾ �̰ɷ� �Ѿ��� ��)
    public Vector3 GetAimDirection()
    {
        return transform.forward;
    }
}
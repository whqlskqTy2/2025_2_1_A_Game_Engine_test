using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                 // �÷��̾� Transform

    [Header("Orbit")]
    public bool holdRightMouseToOrbit = true;
    public float sensX = 120f;               // ���콺 X ����(����)
    public float sensY = 90f;                // ���콺 Y ����(����)
    public float minPitch = 10f;             // �Ʒ��� �ʹ� �� ���̰�
    public float maxPitch = 70f;             // ž�� ���� �Ѱ�
    public float yaw = 0f;                   // ���� ����(�ʱⰪ)
    public float pitch = 35f;                // ���� ����(�ʱⰪ)

    [Header("Zoom")]
    public float distance = 10f;             // �⺻ �Ÿ�
    public float minDistance = 4f;
    public float maxDistance = 18f;
    public float zoomSpeed = 6f;

    [Header("Follow Smoothing")]
    public float followLerp = 12f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float collideRadius = 0.2f;       // ī�޶� �浹 ����(����)

    [Header("Cursor")]
    public bool lockCursorWhenOrbit = false;

    void LateUpdate()
    {
        if (!target) return;

        // --- �Է� �б� (��/�� �ý��� ��� ����) ---
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

        // --- ȸ�� ---
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

        // --- �� ---
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // --- ���ϴ� ��ġ/ȸ�� ��� ---
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position - rot * Vector3.forward * distance;

        // --- �浹 ����(�ɼ�) ---
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

        // --- �ε巴�� ���󰡱� ---
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }
}
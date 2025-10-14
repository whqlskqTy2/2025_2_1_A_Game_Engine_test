using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookController : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensitivity = 240f;     // ���콺 ����(��/��)
    public float smooth = 12f;           // ȸ�� ����(�ε巴��)

    [Header("Pitch Clamp")]
    public float minPitch = -60f;        // �Ʒ���
    public float maxPitch = 80f;        // ����

    [Header("Options")]
    public bool invertY = false;         // ���� ����

    [Header("Player")]
    public Transform playerBody;         // �¿�(Yaw)�� �÷��̾ ȸ��

    [Header("Aim (RMB)")]
    public Camera cam;                   // ���� ī�޶�
    public float normalFOV = 60f;
    public float aimFOV = 42f;
    public float fovLerp = 10f;          // FOV ���� �ӵ�

    float targetYaw;     // ���� ����
    float targetPitch;   // ���� ����

    void Start()
    {
        // �ʱ� ���� ����ȭ
        Vector3 e = playerBody ? playerBody.eulerAngles : Vector3.zero;
        targetYaw = e.y;
        targetPitch = transform.localEulerAngles.x;

        // Ŀ�� ���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!cam) cam = Camera.main;
        if (cam) cam.fieldOfView = normalFOV;
    }

    void Update()
    {
        // ===== �Է� =====
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        if (invertY) mouseY = -mouseY;

        targetYaw += mouseX;
        targetPitch -= mouseY; // ���� �ø��� pitch ������ �ƴ� ���� �������� ��� �� �Ϲ���
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        // ===== ȸ�� ����(�ε巴�� ����) =====
        if (playerBody)
        {
            Quaternion currentYaw = playerBody.rotation;
            Quaternion desiredYaw = Quaternion.Euler(0f, targetYaw, 0f);
            playerBody.rotation = Quaternion.Slerp(currentYaw, desiredYaw, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        }

        Quaternion currentPitch = transform.localRotation;
        Quaternion desiredPitch = Quaternion.Euler(targetPitch, 0f, 0f);
        transform.localRotation = Quaternion.Slerp(currentPitch, desiredPitch, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        // ===== ����(��Ŭ��) FOV =====
        if (cam)
        {
            float targetFov = Input.GetMouseButton(1) ? aimFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, 1f - Mathf.Exp(-fovLerp * Time.deltaTime));
        }

        // ===== Ŀ�� ����/����� =====
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
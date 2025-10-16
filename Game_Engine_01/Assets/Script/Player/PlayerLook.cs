using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Refs")]
    public Transform cameraPivot;   // 1��Ī ī�޶� �޸� Ʈ������(���� ī�޶� �θ�)

    [Header("Mouse Settings")]
    public float mouseSensitivity = 120f;
    public float minPitch = -80f;
    public float maxPitch = 75f;

    float pitch; // ī�޶� ���� ������

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ������ ����(Yaw)
        transform.Rotate(Vector3.up * mouseX);

        // ī�޶�� ����(Pitch) �� ����
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // ���� ȸ�� ���� (x�� �ٲ��ֱ�)
        Vector3 angles = cameraPivot.localEulerAngles;
        // Unity�� 0~360 ���� ü�� ������ ���� ����
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
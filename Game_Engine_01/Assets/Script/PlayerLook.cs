using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Refs")]
    public Transform cameraPivot;   // 1인칭 카메라가 달린 트랜스폼(보통 카메라 부모)

    [Header("Mouse Settings")]
    public float mouseSensitivity = 120f;
    public float minPitch = -80f;
    public float maxPitch = 75f;

    float pitch; // 카메라 상하 누적각

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 몸통은 수평(Yaw)
        transform.Rotate(Vector3.up * mouseX);

        // 카메라는 상하(Pitch) 축 제한
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 로컬 회전 적용 (x만 바꿔주기)
        Vector3 angles = cameraPivot.localEulerAngles;
        // Unity의 0~360 각도 체계 때문에 직접 설정
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
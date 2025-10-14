using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemacineSwither : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public CinemachineFreeLook freeLookCam;

    public bool usingFreeLook = false;

    // 기본 FOV 값
    public float normalFOV = 60f;
    // Shift 눌렀을 때 FOV 값
    public float runFOV = 75f;
    // FOV 전환 속도
    public float fovLerpSpeed = 5f;

    void Start()
    {
        virtualCam.Priority = 10;
        freeLookCam.Priority = 0;

        // 시작 시 기본 FOV 설정
        virtualCam.m_Lens.FieldOfView = normalFOV;
        freeLookCam.m_Lens.FieldOfView = normalFOV;
    }

    void Update()
    {
        // 카메라 전환
        if (Input.GetMouseButtonDown(1))
        {
            usingFreeLook = !usingFreeLook;
            if (usingFreeLook)
            {
                freeLookCam.Priority = 20;
                virtualCam.Priority = 0;
            }
            else
            {
                virtualCam.Priority = 20;
                freeLookCam.Priority = 0;
            }
        }

        // 쉬프트를 누르면 시야각(FOV) 넓히기
        float targetFOV = Input.GetKey(KeyCode.LeftShift) ? runFOV : normalFOV;

        // 현재 활성화된 카메라의 FOV 부드럽게 변경
        if (usingFreeLook)
        {
            freeLookCam.m_Lens.FieldOfView =
                Mathf.Lerp(freeLookCam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        }
        else
        {
            virtualCam.m_Lens.FieldOfView =
                Mathf.Lerp(virtualCam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        }
    }
}

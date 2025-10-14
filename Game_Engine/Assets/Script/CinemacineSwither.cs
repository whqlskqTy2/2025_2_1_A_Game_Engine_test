using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemacineSwither : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public CinemachineFreeLook freeLookCam;

    public bool usingFreeLook = false;

    // �⺻ FOV ��
    public float normalFOV = 60f;
    // Shift ������ �� FOV ��
    public float runFOV = 75f;
    // FOV ��ȯ �ӵ�
    public float fovLerpSpeed = 5f;

    void Start()
    {
        virtualCam.Priority = 10;
        freeLookCam.Priority = 0;

        // ���� �� �⺻ FOV ����
        virtualCam.m_Lens.FieldOfView = normalFOV;
        freeLookCam.m_Lens.FieldOfView = normalFOV;
    }

    void Update()
    {
        // ī�޶� ��ȯ
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

        // ����Ʈ�� ������ �þ߰�(FOV) ������
        float targetFOV = Input.GetKey(KeyCode.LeftShift) ? runFOV : normalFOV;

        // ���� Ȱ��ȭ�� ī�޶��� FOV �ε巴�� ����
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

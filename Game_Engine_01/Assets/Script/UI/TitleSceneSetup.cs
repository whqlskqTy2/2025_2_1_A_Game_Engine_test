using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneSetup : MonoBehaviour
{
    void Start()
    {
        // Ÿ��Ʋ(Ȥ�� ����) �� ������ �� ���콺/�ð� ����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }
}
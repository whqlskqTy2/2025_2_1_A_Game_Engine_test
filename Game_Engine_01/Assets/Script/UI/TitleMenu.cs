using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "GameScene"; // ���� ���� �÷��� �� �̸�

    void Start()
    {
        // Ÿ��Ʋ ȭ�鿡���� Ŀ�� ���̰� �ϰ� ��� Ǯ�� (FPS��)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // "����" ��ư�� ������ �Լ�
    public void OnClickStart()
    {
        // ���� ���� ������ ��ȯ
        SceneManager.LoadScene(gameSceneName);
    }

    // "������" ��ư�� ������ �Լ�
    public void OnClickQuit()
    {
        Debug.Log("Quit requested");

        // ����� ���ӿ����� ���ø����̼� ����
        Application.Quit();

        // �����Ϳ��� �׽�Ʈ�� ���� ������ ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "GameScene"; // 실제 게임 플레이 씬 이름

    void Start()
    {
        // 타이틀 화면에서는 커서 보이게 하고 잠금 풀기 (FPS용)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // "시작" 버튼에 연결할 함수
    public void OnClickStart()
    {
        // 실제 게임 씬으로 전환
        SceneManager.LoadScene(gameSceneName);
    }

    // "나가기" 버튼에 연결할 함수
    public void OnClickQuit()
    {
        Debug.Log("Quit requested");

        // 빌드된 게임에서는 애플리케이션 종료
        Application.Quit();

        // 에디터에서 테스트할 때는 에디터 정지
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
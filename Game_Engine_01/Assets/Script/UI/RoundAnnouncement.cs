using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro를 쓸 거라고 가정

public class RoundAnnouncement : MonoBehaviour
{
    [System.Serializable]
    public class TimedMessage
    {
        public float triggerTime = 10f;     // 몇 초 뒤에 뜰지 (게임 시작 기준)
        public string messageText = "1 라운드";
        public float displayDuration = 3f;  // 화면에 몇 초 동안 유지할지
    }

    [Header("UI Ref")]
    public TextMeshProUGUI messageTextUI;   // "1 라운드" 보여줄 TMP 텍스트
    public CanvasGroup canvasGroup;        // 페이드 in/out 제어용 (없으면 그냥 SetActive 방식으로도 가능)

    [Header("Messages")]
    public TimedMessage[] messages;        // 여러 개 등록 가능. 예: 100초에 "1 라운드", 200초에 "2 라운드"

    [Header("Behavior")]
    public bool loopThroughMessagesOnce = true; // true면 한 번씩만 재생
    public bool fadeInOut = true;
    public float fadeSpeed = 5f;                // 페이드 속도

    private float elapsed;
    private int nextMsgIndex = 0;
    private float hideAtTime = -1f;
    private bool showing = false;

    void Start()
    {
        elapsed = 0f;
        nextMsgIndex = 0;
        hideAtTime = -1f;
        showing = false;

        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
        }
        if (messageTextUI)
        {
            messageTextUI.text = "";
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // 아직 재생 안 한 메시지가 남아 있다면, 다음 메시지의 트리거 타임에 도달했는지 확인
        if (nextMsgIndex < messages.Length)
        {
            var msg = messages[nextMsgIndex];
            if (elapsed >= msg.triggerTime)
            {
                ShowMessage(msg);
                nextMsgIndex++;

                // loopThroughMessagesOnce = false 라면,
                // 나중에 다시 돈다든가 하는 로직을 확장할 수 있음
            }
        }

        // 현재 표시 중이면 시간 보고 숨길지 결정
        if (showing && hideAtTime > 0f && elapsed >= hideAtTime)
        {
            HideMessage();
        }

        // 페이드 처리
        HandleFade();
    }

    void ShowMessage(TimedMessage msg)
    {
        if (!messageTextUI) return;

        messageTextUI.text = msg.messageText;
        hideAtTime = elapsed + msg.displayDuration;
        showing = true;

        if (canvasGroup)
        {
            // 보이게
            if (!fadeInOut)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }

    void HideMessage()
    {
        // 단순히 표시만 끄고, 텍스트는 마지막 문구 유지해도 됨
        showing = false;

        if (!fadeInOut && canvasGroup)
        {
            canvasGroup.alpha = 0f;
        }
    }

    void HandleFade()
    {
        if (!canvasGroup) return;
        if (!fadeInOut) return;

        // showing이면 alpha -> 1, 아니면 alpha -> 0
        float target = showing ? 1f : 0f;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, fadeSpeed * Time.deltaTime);
    }
}
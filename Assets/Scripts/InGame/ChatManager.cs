using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public InputField chatInput; // 채팅 입력 필드
    public Transform contentPanel; // 스크롤뷰의 Content Panel
    public GameObject chatMessagePrefab; // 채팅 메시지 프리팹
    public TeamManager teamManager; // TeamManager 참조
    public ScrollRect scrollRect; // ScrollRect 참조
    public float scrollSensitivity = 10f; // 스크롤 속도 (인스펙터에서 조절 가능)

    private List<string> team1Messages = new List<string>(); // 팀 1 메시지 리스트
    private List<string> team2Messages = new List<string>(); // 팀 2 메시지 리스트

    void Start()
    {
        // 채팅 입력 필드의 이벤트 리스너를 설정합니다.
        chatInput.onEndEdit.AddListener(OnSubmitChat);
        // 스크롤뷰 드래그 속도 설정
        scrollRect.scrollSensitivity = scrollSensitivity;
    }

    void OnSubmitChat(string input)
    {
        if (!string.IsNullOrEmpty(input) && Input.GetKeyDown(KeyCode.Return))
        {
            photonView.RPC("SendChatMessage", RpcTarget.MasterClient, input, PhotonNetwork.LocalPlayer.ActorNumber);
            chatInput.text = ""; // 채팅 입력 필드 비우기
            chatInput.ActivateInputField(); // 입력 필드 활성화 유지
        }
    }

    [PunRPC]
    public void SendChatMessage(string message, int senderActorNumber)
    {
        string senderName = PhotonNetwork.CurrentRoom.GetPlayer(senderActorNumber).NickName;
        string formattedMessage = $"{senderName}: {message}";
        int team = teamManager.Team1.Exists(player => player.ActorNumber == senderActorNumber) ? 1 : 2;

        photonView.RPC("ReceiveChatMessage", RpcTarget.All, formattedMessage, team);
    }

    [PunRPC]
    public void ReceiveChatMessage(string message, int team)
    {
        if (team == 1 && teamManager.Team1.Contains(PhotonNetwork.LocalPlayer))
        {
            team1Messages.Add(message);
            UpdateChatDisplay(team1Messages);
        }
        else if (team == 2 && teamManager.Team2.Contains(PhotonNetwork.LocalPlayer))
        {
            team2Messages.Add(message);
            UpdateChatDisplay(team2Messages);
        }
    }

    private void UpdateChatDisplay(List<string> messagesToDisplay)
    {
        // 기존 메시지 모두 제거
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (string message in messagesToDisplay)
        {
            GameObject newMessage = Instantiate(chatMessagePrefab, contentPanel);
            Text messageText = newMessage.GetComponent<Text>();
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        Canvas.ForceUpdateCanvases(); // 캔버스 강제 업데이트
        StartCoroutine(ScrollToBottom()); // 스크롤뷰를 최신 메시지로 스크롤
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null; // 한 프레임 대기
        scrollRect.verticalNormalizedPosition = 0; // 스크롤뷰를 최신 메시지로 스크롤
    }
}

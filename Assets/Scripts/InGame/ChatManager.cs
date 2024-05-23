using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public InputField chatInput; // ä�� �Է� �ʵ�
    public Transform contentPanel; // ��ũ�Ѻ��� Content Panel
    public GameObject chatMessagePrefab; // ä�� �޽��� ������
    public TeamManager teamManager; // TeamManager ����
    public ScrollRect scrollRect; // ScrollRect ����
    public float scrollSensitivity = 10f; // ��ũ�� �ӵ� (�ν����Ϳ��� ���� ����)

    private List<string> team1Messages = new List<string>(); // �� 1 �޽��� ����Ʈ
    private List<string> team2Messages = new List<string>(); // �� 2 �޽��� ����Ʈ

    void Start()
    {
        // ä�� �Է� �ʵ��� �̺�Ʈ �����ʸ� �����մϴ�.
        chatInput.onEndEdit.AddListener(OnSubmitChat);
        // ��ũ�Ѻ� �巡�� �ӵ� ����
        scrollRect.scrollSensitivity = scrollSensitivity;
    }

    void OnSubmitChat(string input)
    {
        if (!string.IsNullOrEmpty(input) && Input.GetKeyDown(KeyCode.Return))
        {
            photonView.RPC("SendChatMessage", RpcTarget.MasterClient, input, PhotonNetwork.LocalPlayer.ActorNumber);
            chatInput.text = ""; // ä�� �Է� �ʵ� ����
            chatInput.ActivateInputField(); // �Է� �ʵ� Ȱ��ȭ ����
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
        // ���� �޽��� ��� ����
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

        Canvas.ForceUpdateCanvases(); // ĵ���� ���� ������Ʈ
        StartCoroutine(ScrollToBottom()); // ��ũ�Ѻ並 �ֽ� �޽����� ��ũ��
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null; // �� ������ ���
        scrollRect.verticalNormalizedPosition = 0; // ��ũ�Ѻ並 �ֽ� �޽����� ��ũ��
    }
}

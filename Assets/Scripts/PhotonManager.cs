using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;  // Photon ���� ����� ����ϱ� ���� �߰�
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public InputField Name; // Ŭ���̾�Ʈ �̸�
    public Button connectButton; // �����ϱ� ��ư
    private bool isConnected = false;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // ������Ʈ�� �ı����� �ʵ��� ����
    }

    void Start()
    {
        // �����ϱ� ��ư�� Ŭ�� �̺�Ʈ �߰�
        connectButton.onClick.AddListener(OnConnectButtonClicked);
    }

    // �����ϱ� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnConnectButtonClicked()
    {
        if (!isConnected)
        {
            // Ŭ���̾�Ʈ �̸� ����
            string playerName = Name.text;
            if (string.IsNullOrEmpty(playerName))
            {
                Debug.LogError("Player name is invalid.");
                return;
            }

            PhotonNetwork.NickName = playerName;

            // Photon ������ ����
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ���� ������ �������� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server with ID: " + PhotonNetwork.LocalPlayer.UserId);

        Debug.Log("Connected to Photon Server");
        isConnected = true;
        // �κ� ����
        PhotonNetwork.JoinLobby();
    }

    // �κ� ������ �������� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        // �κ� ������ �̵�
        SceneManager.LoadScene("LobbyScene"); // �κ������ �̵�
    }

    // ���� ���ӿ� �������� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"���漭���� ����Ǿ����ϴ�: {cause.ToString()}");
        isConnected = false;
    }

    public override void OnJoinedRoom()
    {
        
        Debug.Log("Joined a room successfully.");

        // ���⿡�� �÷��̾� ��ü�� ������ �� �ֽ��ϴ�.
        // PhotonNetwork.Instantiate�� ����Ͽ� ��� Ŭ���̾�Ʈ���� ����ȭ�� ��ü ����
        Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        GameObject playerObject = PhotonNetwork.Instantiate("PlayerPrefab", spawnPosition, Quaternion.identity);

        if (playerObject != null)
        {
            Debug.Log("PlayerPrefab instantiated successfully.");
        }
        else
        {
            Debug.LogError("PlayerPrefab instantiation failed.");
        }
    }

}

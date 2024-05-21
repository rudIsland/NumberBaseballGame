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
    public Player player;
    private static HashSet<int> usedIDs = new HashSet<int>(); // ���� ID�� �����ϱ� ���� HashSet

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
            PhotonNetwork.NickName = Name.text;

            // ���� ID ����
            int uniqueID = GenerateUniqueID();
            Player client = new Player(GenerateUniqueID(), Name.text); //�÷��̾ ������.

            // Photon ������ ����
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ���� ������ �������� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
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

    // �ߺ����� �ʴ� ���� ID�� �����ϴ� �Լ�
    private int GenerateUniqueID()
    {
        int newID;
        do
        {
            newID = Random.Range(1, 101);
        } while (usedIDs.Contains(newID));

        usedIDs.Add(newID);
        return newID;
    }
}

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;  // Photon ���� ����� ����ϱ� ���� �߰�
using Photon.Realtime;
using UnityEngine.SceneManagement;

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
            PhotonNetwork.NickName = Name.text;
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
}

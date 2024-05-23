using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomNameInput; // �� �̸� �Է� �ʵ�
    public GameObject roomButtonPrefab; // �� ��ư ������
    public Transform roomListContent; // �� ����� ǥ���� �θ� ��ü
    private string selectedRoomName; // ���õ� �� �̸�

    public Button CreateRoomBtn; // �� �����ϱ� ��ư
    public Button ConnectGameRoomBtn; // �� �����ϱ� ��ư

    public GameObject CreateRoomPanel; // �� ���� �ǳ�
    public GameObject EnterRoomPanel; // �����ϱ� �ǳ�

    private Dictionary<string, RoomInfo> cachedRoomList; // �� ��� ĳ��

    void Start()
    {
        // Photon ������ �̹� ����� ��� �ٽ� �������� ����
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        CreateRoomPanel.SetActive(false);
        EnterRoomPanel.SetActive(false); // �����ϱ� �г� ����

        // ��ư Ŭ�� �̺�Ʈ ����
        CreateRoomBtn.onClick.AddListener(OnCreateRoomButtonClicked);
        ConnectGameRoomBtn.onClick.AddListener(OnJoinRoomButtonClicked);

        cachedRoomList = new Dictionary<string, RoomInfo>();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon ������ ����Ǿ����ϴ�.");
        // �κ� ����
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� �����Ͽ����ϴ�.");
        // �κ� �������� �� �ʱ� �� ����� ������Ʈ
        UpdateRoomList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // �� ����� ������Ʈ�Ǹ� ĳ�ÿ� UI�� ������Ʈ
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                cachedRoomList.Remove(roomInfo.Name);
            }
            else
            {
                cachedRoomList[roomInfo.Name] = roomInfo;
            }
        }
        UpdateRoomList();
    }

    void UpdateRoomList()
    {
        // ���� �� ��� ����
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo roomInfo in cachedRoomList.Values)
        {
            GameObject roomButtonObj = Instantiate(roomButtonPrefab, roomListContent);
            RoomButton roomButton = roomButtonObj.GetComponent<RoomButton>();
            roomButton.Setup(roomInfo, this);
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInput.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
        }
    }

    public void OnRoomButtonClicked(string roomName)
    {
        selectedRoomName = roomName;
    }

    public void OnJoinRoomButtonClicked()
    {
        if (!string.IsNullOrEmpty(selectedRoomName))
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRoom(selectedRoomName);
            }
            else
            {
                Debug.LogWarning("Ŭ���̾�Ʈ�� �濡 ������ �غ� ���� �ʾҽ��ϴ�.");
                StartCoroutine(RetryJoinRoom(selectedRoomName));
            }
        }
        else
        {
            Debug.LogWarning("������ ���� ���õ��� �ʾҽ��ϴ�.");
        }
    }

    private IEnumerator RetryJoinRoom(string roomName)
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return new WaitForSeconds(1);
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("���� �����Ǿ����ϴ�: " + PhotonNetwork.CurrentRoom.Name);
        CloseCreateRoomPanel(); // ���� �����Ǹ� �� ���� �ǳ��� ����
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�濡 �����Ͽ����ϴ�: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("GameRoom");
    }

    public void CreateRoom() // �� �����ϱ� �ǳ� ����
    {
        if (!CreateRoomPanel.activeSelf)
        {
            CreateRoomPanel.SetActive(true);
        }
    }

    public void CloseCreateRoomPanel()
    {
        if (CreateRoomPanel.activeSelf)
        {
            CreateRoomPanel.SetActive(false);
        }
    }

    public void CloseEnterRoomPanel() // �����ϱ� �г� �ݱ�
    {
        if (EnterRoomPanel.activeSelf)
        {
            EnterRoomPanel.SetActive(false);
        }
    }

    public void OpenEnterRoomPanel() // �����ϱ� �г� ����
    {
        if (!EnterRoomPanel.activeSelf)
        {
            EnterRoomPanel.SetActive(true);
        }
    }
}

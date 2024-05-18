using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomNameInput; // �� �̸� �Է� �ʵ�
    public GameObject roomButtonPrefab; // �� ��ư ������
    public Transform roomListContent; // �� ����� ǥ���� �θ� ��ü
    private string selectedRoomName; // ���õ� �� �̸�

    public Button CreateRoomBtn; // �� �����ϱ� ��ư
    public Button ConnectGameRoomBtn; // �� �����ϱ� ��ư

    public GameObject CreateRoomPannel; // �� ���� �ǳ�

    void Start()
    {
        // Photon ������ �̹� ����� ��� �ٽ� �������� ����
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        CreateRoomPannel.SetActive(false);

        // ��ư Ŭ�� �̺�Ʈ ����
        CreateRoomBtn.onClick.AddListener(OnCreateRoomButtonClicked);
        ConnectGameRoomBtn.onClick.AddListener(OnJoinRoomButtonClicked);
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
        // �� ����� ������Ʈ�Ǹ� UI�� ������Ʈ
        UpdateRoomList(roomList);
    }

    void UpdateRoomList(List<RoomInfo> roomList = null)
    {
        // ���� �� ��� ����
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        if (roomList != null)
        {
            foreach (RoomInfo roomInfo in roomList)
            {
                GameObject roomButtonObj = Instantiate(roomButtonPrefab, roomListContent);
                RoomButton roomButton = roomButtonObj.GetComponent<RoomButton>();
                roomButton.Setup(roomInfo, this);
            }
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
            }
        }
        else
        {
            Debug.LogWarning("������ ���� ���õ��� �ʾҽ��ϴ�.");
        }
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
        if (!CreateRoomPannel.activeSelf)
        {
            CreateRoomPannel.SetActive(true);
        }
    }

    public void CloseCreateRoomPanel()
    {
        if (CreateRoomPannel.activeSelf)
        {
            CreateRoomPannel.SetActive(false);
        }
    }
}

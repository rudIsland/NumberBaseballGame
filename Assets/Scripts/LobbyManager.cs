using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomNameInput; // 방 이름 입력 필드
    public GameObject roomButtonPrefab; // 방 버튼 프리팹
    public Transform roomListContent; // 방 목록을 표시할 부모 객체
    private string selectedRoomName; // 선택된 방 이름

    public Button CreateRoomBtn; // 방 생성하기 버튼
    public Button ConnectGameRoomBtn; // 방 입장하기 버튼

    public GameObject CreateRoomPannel; // 방 생성 판넬

    void Start()
    {
        // Photon 서버에 이미 연결된 경우 다시 연결하지 않음
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        CreateRoomPannel.SetActive(false);

        // 버튼 클릭 이벤트 설정
        CreateRoomBtn.onClick.AddListener(OnCreateRoomButtonClicked);
        ConnectGameRoomBtn.onClick.AddListener(OnJoinRoomButtonClicked);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon 서버에 연결되었습니다.");
        // 로비에 접속
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 접속하였습니다.");
        // 로비에 접속했을 때 초기 방 목록을 업데이트
        UpdateRoomList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방 목록이 업데이트되면 UI도 업데이트
        UpdateRoomList(roomList);
    }

    void UpdateRoomList(List<RoomInfo> roomList = null)
    {
        // 기존 방 목록 제거
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
                Debug.LogWarning("클라이언트가 방에 입장할 준비가 되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning("입장할 방이 선택되지 않았습니다.");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방이 생성되었습니다: " + PhotonNetwork.CurrentRoom.Name);
        CloseCreateRoomPanel(); // 방이 생성되면 방 생성 판넬을 닫음
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방에 입장하였습니다: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("GameRoom");
    }

    public void CreateRoom() // 방 생성하기 판넬 열기
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

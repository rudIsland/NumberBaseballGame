using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomNameInput; // 방 이름 입력 필드
    public GameObject roomButtonPrefab; // 방 버튼 프리팹
    public Transform roomListContent; // 방 목록을 표시할 부모 객체
    private string selectedRoomName; // 선택된 방 이름

    public Button CreateRoomBtn; // 방 생성하기 버튼
    public Button ConnectGameRoomBtn; // 방 입장하기 버튼

    public GameObject CreateRoomPanel; // 방 생성 판넬
    public GameObject EnterRoomPanel; // 입장하기 판넬

    private Dictionary<string, RoomInfo> cachedRoomList; // 방 목록 캐시

    void Start()
    {
        // Photon 서버에 이미 연결된 경우 다시 연결하지 않음
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        CreateRoomPanel.SetActive(false);
        EnterRoomPanel.SetActive(false); // 입장하기 패널 끄기

        // 버튼 클릭 이벤트 설정
        CreateRoomBtn.onClick.AddListener(OnCreateRoomButtonClicked);
        ConnectGameRoomBtn.onClick.AddListener(OnJoinRoomButtonClicked);

        cachedRoomList = new Dictionary<string, RoomInfo>();
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
        // 방 목록이 업데이트되면 캐시와 UI도 업데이트
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
        // 기존 방 목록 제거
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
                Debug.LogWarning("클라이언트가 방에 입장할 준비가 되지 않았습니다.");
                StartCoroutine(RetryJoinRoom(selectedRoomName));
            }
        }
        else
        {
            Debug.LogWarning("입장할 방이 선택되지 않았습니다.");
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

    public void CloseEnterRoomPanel() // 입장하기 패널 닫기
    {
        if (EnterRoomPanel.activeSelf)
        {
            EnterRoomPanel.SetActive(false);
        }
    }

    public void OpenEnterRoomPanel() // 입장하기 패널 열기
    {
        if (!EnterRoomPanel.activeSelf)
        {
            EnterRoomPanel.SetActive(true);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;  // Photon 관련 기능을 사용하기 위해 추가
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public InputField Name; // 클라이언트 이름
    public Button connectButton; // 접속하기 버튼
    private bool isConnected = false;
    public Player player;
    private static HashSet<int> usedIDs = new HashSet<int>(); // 사용된 ID를 추적하기 위한 HashSet

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // 오브젝트가 파괴되지 않도록 설정
    }

    void Start()
    {
        // 접속하기 버튼에 클릭 이벤트 추가
        connectButton.onClick.AddListener(OnConnectButtonClicked);
    }

    // 접속하기 버튼 클릭 시 호출되는 함수
    public void OnConnectButtonClicked()
    {
        if (!isConnected)
        {
            // 클라이언트 이름 설정
            PhotonNetwork.NickName = Name.text;

            // 고유 ID 설정
            int uniqueID = GenerateUniqueID();
            Player client = new Player(GenerateUniqueID(), Name.text); //플레이어가 생성됨.

            // Photon 서버에 접속
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 서버 접속이 성공했을 때 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server");
        isConnected = true;
        // 로비에 접속
        PhotonNetwork.JoinLobby();
    }

    // 로비에 접속이 성공했을 때 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        // 로비 씬으로 이동
        SceneManager.LoadScene("LobbyScene"); // 로비씬으로 이동
    }

    // 서버 접속에 실패했을 때 호출되는 콜백 함수
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"포톤서버가 종료되었습니다: {cause.ToString()}");
        isConnected = false;
    }

    // 중복되지 않는 고유 ID를 생성하는 함수
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

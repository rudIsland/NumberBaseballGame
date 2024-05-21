using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // 포톤 서버에 연결
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default); // 룸에 입장 또는 생성
    }

    public override void OnJoinedRoom()
    {
        // 룸에 입장한 후 게임 오브젝트 생성
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("GameManager", Vector3.zero, Quaternion.identity);
        }
    }
}
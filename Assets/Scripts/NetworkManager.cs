using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // ���� ������ ����
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default); // �뿡 ���� �Ǵ� ����
    }

    public override void OnJoinedRoom()
    {
        // �뿡 ������ �� ���� ������Ʈ ����
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("GameManager", Vector3.zero, Quaternion.identity);
        }
    }
}
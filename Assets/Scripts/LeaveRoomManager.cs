using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveRoomManager : MonoBehaviourPunCallbacks
{
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // 방을 떠남
    }
    public override void OnLeftRoom()
    {
        Debug.Log("방을 떠났습니다.");
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby(); // 로비에 접속
        }
        else
        {
            Debug.LogError("Photon 서버에 연결되지 않았습니다.");
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 접속하였습니다.");
        SceneManager.LoadScene("LobbyScene"); // 로비 씬으로 이동
    }
}

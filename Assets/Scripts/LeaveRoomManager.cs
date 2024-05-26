using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveRoomManager : MonoBehaviourPunCallbacks
{
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // ���� ����
    }
    public override void OnLeftRoom()
    {
        Debug.Log("���� �������ϴ�.");
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby(); // �κ� ����
        }
        else
        {
            Debug.LogError("Photon ������ ������� �ʾҽ��ϴ�.");
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� �����Ͽ����ϴ�.");
        SceneManager.LoadScene("LobbyScene"); // �κ� ������ �̵�
    }
}

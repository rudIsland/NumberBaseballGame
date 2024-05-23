using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public Text GameLog;
    public List<Photon.Realtime.Player> RoomPlayerList = new List<Photon.Realtime.Player>();
    public TeamManager teamManager;

    void Start()
    {
        // �濡 ó�� �� �÷��̾�(����)�� ����Ʈ�� �߰�
        if (PhotonNetwork.IsMasterClient)
        {
            addNewPlayerToList(PhotonNetwork.LocalPlayer);

            photonView.RPC("BroadcastLog", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " entered the room.");
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " �� ����");
        photonView.RPC("BroadcastLog", RpcTarget.All, newPlayer.NickName + " Join the room.");
        addNewPlayerToList(newPlayer);
        // ���� ���� �÷��̾�� ���� ���� ��� �÷��̾� ������ ������ UI�� ������Ʈ�մϴ�.
        photonView.RPC("SyncPlayerList", newPlayer, PhotonNetwork.PlayerList);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " �� ����");
        photonView.RPC("BroadcastLog", RpcTarget.All, otherPlayer.NickName + " left the room.");
        removePlayerFromList(otherPlayer);
    }

    //����Ʈ�� �÷��̾� �߰�
    public void addNewPlayerToList(Photon.Realtime.Player player)
    {
        RoomPlayerList.Add(player);
        teamManager.AssignTeam(player);
    }
    
    //����Ʈ���� �÷��̾� ����
    public void removePlayerFromList(Photon.Realtime.Player player)
    {
        RoomPlayerList.Remove(player);
        teamManager.RemovePlayerFromTeam(player);
    }

    [PunRPC]
    public void BroadcastLog(string message) //��ο��� �α� �޽���
    {
        Debug.Log(message);
        GameLog.text += message + "\n";
    }

    [PunRPC]
    public void SyncPlayerList(Photon.Realtime.Player[] players)
    {
        foreach (Photon.Realtime.Player player in players)
        {
            if (!RoomPlayerList.Contains(player))
            {
                addNewPlayerToList(player);
            }
        }
    }

    public override void OnJoinedRoom() //������
    {
        // �濡 �����ϸ� �ڽ��� �÷��̾� ��ü�� �����ϰ� ����Ʈ�� �߰�
        Debug.Log("Joined a room successfully.");
        RoomPlayerList.Clear(); // �ߺ� ������ ���� ����Ʈ �ʱ�ȭ
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (!RoomPlayerList.Contains(player))
            {
                addNewPlayerToList(player);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using Photon.Pun.Demo.PunBasics;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public Text GameLog;
    public List<Photon.Realtime.Player> RoomPlayerList = new List<Photon.Realtime.Player>();
    public List<Photon.Realtime.Player> ReadyPlayers = new List<Photon.Realtime.Player>(); // 준비 상태인 플레이어 리스트
    public TeamManager teamManager;
    public GameManager gameManager;



    void Start()
    {
        // 방에 처음 들어간 플레이어(방장)도 리스트에 추가
        if (PhotonNetwork.IsMasterClient)
        {
            addNewPlayerToList(PhotonNetwork.LocalPlayer);
            photonView.RPC("BroadcastLog", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " entered the room.");
            gameManager.UpdateStartGameButtonVisibility();
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " 이 입장");
        photonView.RPC("BroadcastLog", RpcTarget.All, newPlayer.NickName + " Join the room.");
        addNewPlayerToList(newPlayer);
        // 새로 들어온 플레이어에게 현재 방의 모든 플레이어 정보를 보내서 UI를 업데이트합니다.
        photonView.RPC("SyncPlayerList", newPlayer, PhotonNetwork.PlayerList);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " 이 퇴장");
        photonView.RPC("BroadcastLog", RpcTarget.All, otherPlayer.NickName + " left the room.");
        removePlayerFromList(otherPlayer);
        if (PhotonNetwork.IsMasterClient)
        {
            AssignNewMasterClient();
        }
    }

    //리스트에 플레이어 추가
    public void addNewPlayerToList(Photon.Realtime.Player player)
    {
        RoomPlayerList.Add(player);
        teamManager.AssignTeam(player);
     }
    
    //리스트에서 플레이어 제거
    public void removePlayerFromList(Photon.Realtime.Player player)
    {
        RoomPlayerList.Remove(player);
        ReadyPlayers.Remove(player);
        teamManager.RemovePlayerFromTeam(player);
    }

    [PunRPC]
    public void BroadcastLog(string message) //모두에게 로그 메시지
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

    public override void OnJoinedRoom() //방입장
    {
        // 방에 참가하면 자신의 플레이어 객체를 생성하고 리스트에 추가
        Debug.Log("Joined a room successfully.");
        RoomPlayerList.Clear(); // 중복 방지를 위해 리스트 초기화
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (!RoomPlayerList.Contains(player))
            {
                addNewPlayerToList(player);
            }
        }
    }

    // 새로운 방장을 설정하는 메서드
    public void AssignNewMasterClient()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (teamManager.Team1.Count > 0)
        {
            PhotonNetwork.SetMasterClient(teamManager.Team1[0]);
        }
        else if (teamManager.Team2.Count > 0)
        {
            PhotonNetwork.SetMasterClient(teamManager.Team2[0]);
        }

        gameManager.UpdateStartGameButtonVisibility();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        gameManager.UpdateStartGameButtonVisibility();
    }

}

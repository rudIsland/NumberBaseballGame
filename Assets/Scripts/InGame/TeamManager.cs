using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamManager : MonoBehaviourPunCallbacks
{
    public Text GameLog; //게임 로그
    public List<Photon.Realtime.Player> Team1 = new List<Photon.Realtime.Player>();
    public List<Photon.Realtime.Player> Team2 = new List<Photon.Realtime.Player>();

    // 팀 패널 UI
    public Transform team1Panel;
    public Transform team2Panel;

    // PlayerUI 프리팹
    public GameObject playerUIPrefab;
    private Dictionary<Photon.Realtime.Player, GameObject> playerUIs = new Dictionary<Photon.Realtime.Player, GameObject>();

    public void AssignTeam(Photon.Realtime.Player player)
    {
        if (Team1.Count <= Team2.Count)
        {
            Team1.Add(player);
            Debug.Log(player.NickName + " assigned to Team 1");
            GameLog.text += player.NickName + " assigned to Team 1\n";
            AddPlayerToUI(player, team1Panel);
        }
        else
        {
            Team2.Add(player);
            Debug.Log(player.NickName + " assigned to Team 2");
            GameLog.text += player.NickName + " assigned to Team 2\n";
            AddPlayerToUI(player, team2Panel);
        }
    }

    private void AddPlayerToUI(Photon.Realtime.Player player, Transform teamPanel)
    {
        GameObject playerUI = Instantiate(playerUIPrefab, teamPanel);
        playerUI.GetComponentInChildren<Text>().text = player.NickName;

        playerUIs[player] = playerUI;
    }


    //팀에서 플레이어 제거
    public void RemovePlayerFromTeam(Photon.Realtime.Player player)
    {
        if (Team1.Contains(player))
        {
            Team1.Remove(player);
        }
        else if (Team2.Contains(player))
        {
            Team2.Remove(player);
        }

        if (playerUIs.ContainsKey(player))
        {
            Destroy(playerUIs[player]);
            playerUIs.Remove(player);
        }
    }

    // 팀 변경
    [PunRPC]
        public void SwitchPlayerTeamRPC(int playerId, int toTeamIndex)
    {
        Photon.Realtime.Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerId);
        if (player == null)
        {
            return;
        }

        List<Photon.Realtime.Player> fromTeam = Team1.Contains(player) ? Team1 : Team2;
        List<Photon.Realtime.Player> toTeam = toTeamIndex == 1 ? Team1 : Team2;
        Transform newTeamPanel = toTeamIndex == 1 ? team1Panel : team2Panel;

        if (fromTeam != toTeam)
        {
            fromTeam.Remove(player);
            toTeam.Add(player);

            Debug.Log(player.NickName + " switched to Team " + toTeamIndex);
            GameLog.text += player.NickName + " switched to Team " + toTeamIndex + "\n";

            UpdatePlayerUI(player, newTeamPanel);
        }
    }

    public void SwitchPlayerTeam(Photon.Realtime.Player player, int toTeamIndex)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        photonView.RPC("SwitchPlayerTeamRPC", RpcTarget.All, player.ActorNumber, toTeamIndex);
    }

    private void UpdatePlayerUI(Photon.Realtime.Player player, Transform newTeamPanel)
    {
        if (playerUIs.ContainsKey(player))
        {
            playerUIs[player].transform.SetParent(newTeamPanel, false);
        }
    }

    // UI에서 플레이어를 가져오는 메서드
    public Photon.Realtime.Player GetPlayerByUI(GameObject playerUI)
    {
        foreach (var kvp in playerUIs)
        {
            if (kvp.Value == playerUI)
            {
                return kvp.Key;
            }
        }
        return null;
    }
}
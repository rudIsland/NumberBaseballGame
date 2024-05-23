using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections.Generic;
using UnityEngine;
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
        Text playerNameText = playerUI.GetComponentInChildren<Text>();
        if (playerNameText != null)
        {
            playerNameText.text = player.NickName;
        }
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
}

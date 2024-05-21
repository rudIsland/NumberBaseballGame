using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviourPunCallbacks
{
    public Team team1;
    public Team team2;
    public GameObject playerPrefab; // PlayerUI 프리팹
    public Transform team1Panel;
    public Transform team2Panel;

    public void AddPlayerToTeam(Player player)
    {
        if (player != null)
        {
            if (team1.GetPlayers().Count <= team2.GetPlayers().Count)
            {
                team1.AddPlayer(player);
                UpdateTeamUI(team1Panel, team1.GetPlayers());
            }
            else
            {
                team2.AddPlayer(player);
                UpdateTeamUI(team2Panel, team2.GetPlayers());
            }
        }
        else
        {
            Debug.Log("추가될 플레이어가 널임");
        }
    }

    public void RemovePlayerFromTeam(int uniqueID)
    {
        if (team1.HasPlayer(uniqueID))
        {
            team1.RemovePlayer(uniqueID);
            UpdateTeamUI(team1Panel, team1.GetPlayers());
        }
        else if (team2.HasPlayer(uniqueID))
        {
            team2.RemovePlayer(uniqueID);
            UpdateTeamUI(team2Panel, team2.GetPlayers());
        }
    }

    public void UpdateTeamUI(Transform teamPanel, List<Player> players)
    {
        // 기존 UI 요소 제거
        foreach (Transform child in teamPanel)
        {
            Destroy(child.gameObject);
        }

        // 리스트의 각 플레이어에 대해 UI 요소 생성
        foreach (Player player in players)
        {
            GameObject playerUIObject = Instantiate(playerPrefab, teamPanel);
            PlayerUI playerUI = playerUIObject.GetComponent<PlayerUI>();
            playerUI.SetPlayerName(player.getName());
        }
    }

    internal void AddPlayerToTeam(Photon.Realtime.Player newPlayer)
    {
        throw new NotImplementedException();
    }
}

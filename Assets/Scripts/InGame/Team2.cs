using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team2 : MonoBehaviour
{
    private List<Player> PlayerList = new List<Player>(); // 플레이어 리스트
    public GameObject playerPrefab; // PlayerUI 프리팹
    public Transform teamPanel;

    void UpdateUI()
    {
        // 기존 UI 요소 제거
        foreach (Transform child in teamPanel)
        {
            Destroy(child.gameObject);
        }

        // 리스트의 각 플레이어에 대해 UI 요소 생성
        foreach (Player player in PlayerList)
        {
            GameObject playerUIObject = Instantiate(playerPrefab, teamPanel);
            PlayerUI playerUI = playerUIObject.GetComponent<PlayerUI>();
            playerUI.SetPlayerName(player.getName());
        }
    }

    public void AddPlayer(Player player)
    {
        if (!PlayerList.Exists(p => p.getName() == player.getName()))
        {
            PlayerList.Add(player);
            UpdateUI();
        }
    }

    public List<Player> GetPlayers()
    {
        return PlayerList;
    }
    public void ClearPlayers()
    {
        PlayerList.Clear();
        UpdateUI();
    }

    public void RemovePlayer(Player remove_Player)
    {
        PlayerList.RemoveAll(player => player.getName() == remove_Player.getName());
        UpdateUI();
    }
}

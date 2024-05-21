using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team1 : MonoBehaviour
{
    private List<Player> playerList = new List<Player>(); // 플레이어 리스트

    public void AddPlayer(Player player)
    {
        if (!playerList.Exists(p => p.getName() == player.getName()))
        {
            playerList.Add(player);
        }
    }

    public List<Player> GetPlayers()
    {
        return playerList;
    }

    public bool HasPlayer(int uniqueID)
    {
        return playerList.Exists(player => player.getUniqueID() == uniqueID);
    }

    public void RemovePlayer(int uniqueID)
    {
        Player playerToRemove = playerList.Find(player => player.getUniqueID() == uniqueID);
        if (playerToRemove != null)
        {
            playerList.Remove(playerToRemove);
        }
    }
}

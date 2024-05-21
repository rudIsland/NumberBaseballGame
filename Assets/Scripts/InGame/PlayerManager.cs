using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public TeamManager teamManager;

    private List<Player> playerList = new List<Player>();

    public void AddNewPlayer(Photon.Realtime.Player newPlayer)
    {
        if (teamManager != null)
        {
            Debug.Log("�÷��̾� ����." + newPlayer.NickName);
            teamManager.AddPlayerToTeam(new Player(newPlayer.ActorNumber, newPlayer.NickName));
            playerList.Add(new Player(newPlayer.ActorNumber, newPlayer.NickName));
        }
        else
        {
            Debug.LogWarning("TeamManager�� �������� �ʾҽ��ϴ�.");
        }
    }

    public void RemovePlayer(Photon.Realtime.Player player)
    {
        if (teamManager != null)
        {
            teamManager.RemovePlayerFromTeam(player.ActorNumber);
            playerList.RemoveAll(p => p.getUniqueID() == player.ActorNumber);
            
        }
    }
}

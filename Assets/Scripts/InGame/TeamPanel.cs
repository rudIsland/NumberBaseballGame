using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPanel : MonoBehaviour
{
    public TeamManager teamManager;
    public int teamIndex; // 1 �Ǵ� 2�� ����

    public void OnPlayerDropped(GameObject playerUI)
    {
        Photon.Realtime.Player player = teamManager.GetPlayerByUI(playerUI);
        if (player != null)
        {
            teamManager.SwitchPlayerTeam(player, teamIndex);
        }
    }
}

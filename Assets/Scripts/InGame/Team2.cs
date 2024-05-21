using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team2 : MonoBehaviour
{
    private List<Player> PlayerList = new List<Player>(); // �÷��̾� ����Ʈ
    public GameObject playerPrefab; // PlayerUI ������
    public Transform teamPanel;

    void UpdateUI()
    {
        // ���� UI ��� ����
        foreach (Transform child in teamPanel)
        {
            Destroy(child.gameObject);
        }

        // ����Ʈ�� �� �÷��̾ ���� UI ��� ����
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

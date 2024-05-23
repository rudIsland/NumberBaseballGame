using Photon.Pun;
using UnityEngine;


public class Player : MonoBehaviourPun
{
    public string playerName;
    public string playerId; // ���� ID �߰�


    void Start()
    {
        if (photonView.IsMine)
        {
            playerName = PhotonNetwork.LocalPlayer.NickName;
            playerId = PhotonNetwork.LocalPlayer.UserId;

            // �α׷� �÷��̾� ���� ���
            Debug.Log("Player Name: " + playerName);
            Debug.Log("Player ID: " + playerId);
        }
    }

    public Player(string unique, string name)
    {
        playerId = unique;
        playerName = name;
    }

    public void setName(string name)
    {
        playerName = name;
    }

    public string getName()
    {
        return playerName;
    }


    public void setplayerId(string id)
    {
        playerId = id;
    }

    public string getplayerId()
    {
        return playerId;
    }
}

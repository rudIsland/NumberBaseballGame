using Photon.Pun;
using UnityEngine;


public class Player : MonoBehaviourPun
{
    public string playerName;
    public string playerId; // 고유 ID 추가


    void Start()
    {
        if (photonView.IsMine)
        {
            playerName = PhotonNetwork.LocalPlayer.NickName;
            playerId = PhotonNetwork.LocalPlayer.UserId;

            // 로그로 플레이어 정보 출력
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

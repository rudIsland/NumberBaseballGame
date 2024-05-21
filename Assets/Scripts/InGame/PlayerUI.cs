using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text playerName;
    private Player Player;

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }
}

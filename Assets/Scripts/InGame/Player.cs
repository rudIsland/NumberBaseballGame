using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public int teamNumber;
    public int uniqueID; // ���� ID �߰�

    public Player(int unique, string name)
    {
        uniqueID = unique;
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

    public void setTeamNumber(int num)
    {
        teamNumber = num;
    }

    public int getTeamNumber()
    {
        return teamNumber;
    }

    public void setUniqueID(int id)
    {
        uniqueID = id;
    }

    public int getUniqueID()
    {
        return uniqueID;
    }
}

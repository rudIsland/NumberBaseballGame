using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Text GameLog;
    void Start()
    {
        // �ʿ��� �ʱ�ȭ �۾� ����
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entered the room.");
        GameLog.text += newPlayer.NickName + " entered the room.\n";
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " left the room.");
        GameLog.text += otherPlayer.NickName + " left the room.";
    }
}

using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    public Text roomNameText;
    private LobbyManager lobbyManager;
    private string roomName;

    public void Setup(RoomInfo roomInfo, LobbyManager manager)
    {
        roomName = roomInfo.Name;
        roomNameText.text = roomName;
        lobbyManager = manager;
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        lobbyManager.OnRoomButtonClicked(roomName);
    }
}

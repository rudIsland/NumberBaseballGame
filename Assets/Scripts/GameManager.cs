using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Text GameLog;
    private string secretNumber; // ��� ��ȣ�� �����ϴ� ����
    private List<string> guesses = new List<string>(); // �÷��̾��� ������ �����ϴ� ����Ʈ

    public PlayerManager playerManager; // TeamManager�� ����

    void Start()
    {
        GameStart();
    }

    void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateSecretNumber(); // ��� ��ȣ ����
        }
    }

    void GenerateSecretNumber()
    {
        // 0-9 ������ �ߺ����� �ʴ� 4�ڸ� ���ڸ� ����
        List<int> digits = new List<int>();
        while (digits.Count < 4)
        {
            int randomDigit = Random.Range(0, 10);
            if (!digits.Contains(randomDigit))
            {
                digits.Add(randomDigit);
            }
        }
        secretNumber = string.Join("", digits); // ����Ʈ�� ���ڿ��� ��ȯ�Ͽ� ��� ��ȣ ����
        Debug.Log(secretNumber);
    }

    public string CheckGuess(string guess)
    {
        int strikes = 0; // ��Ʈ����ũ ��
        int balls = 0; // �� ��

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == secretNumber[i]) // ���� ��ġ�� ���� ���ڰ� �ִ� ���
            {
                strikes++;
            }
            else if (secretNumber.Contains(guess[i].ToString())) // �ٸ� ��ġ�� ���� ���ڰ� �ִ� ���
            {
                balls++;
            }
        }

        guesses.Add(guess); // ������ ����Ʈ�� �߰�
        return $"{strikes}S {balls}B"; // ��� ��ȯ
    }

    [PunRPC]
    public void SubmitGuess(string guess, PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string result = CheckGuess(guess); // ������ Ȯ���Ͽ� ��� ���
            photonView.RPC("ReceiveResult", RpcTarget.All, info.Sender.NickName, guess, result); // ��� Ŭ���̾�Ʈ���� ��� ����
        }
    }

    [PunRPC]
    void ReceiveResult(string senderName, string guess, string result)
    {
        // ����� UI�� ǥ���ϴ� ���� (�ӽ÷� ����� �α� ���)
        Debug.Log($"{senderName} guessed {guess}, Result: {result}");
        // UI �Ŵ����� DisplayResult �޼��带 ȣ���Ͽ� ����� ǥ��
        UIManager.Instance.DisplayResult($"{guess}: {result}");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entered the room.");
        GameLog.text += newPlayer.NickName + " entered the room.\n";

        if (playerManager != null)
        {
            Debug.Log(newPlayer.NickName + " entered the room.");
            // �÷��̾� �Ŵ����� ���� ���ο� �÷��̾� ���� ����
            playerManager.AddNewPlayer(newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " left the room.");
        GameLog.text += otherPlayer.NickName + " left the room.";

        if (playerManager != null)
        {
            // �÷��̾� �Ŵ����� ���� �÷��̾� ���� ���� ����
            playerManager.RemovePlayer(otherPlayer);
        }
    }

    [PunRPC]
    public void SubmitGuess(string guess, int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string result = CheckGuess(guess); // ������ Ȯ���Ͽ� ��� ���
            photonView.RPC("ReceiveResult", RpcTarget.All, playerId, guess, result); // ��� Ŭ���̾�Ʈ���� ��� ����
        }
    }
}

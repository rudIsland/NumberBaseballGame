using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Text GameLog;
    private string secretNumber; // ��� ��ȣ�� �����ϴ� ����
    private List<string> guesses = new List<string>(); // �÷��̾��� ������ �����ϴ� ����Ʈ

    public PlayerManager playerManager; // playerManager�� ����
    public TeamManager teamManager; //TeamManager�� ����

    private bool isGameStarting = false;
    private int readyPlayersCount = 0;
    public Text countdownText; //ī��Ʈ�ٿ� �ؽ�Ʈ
    public Button readyButton;

    void Start()
    {
        UIManager.Instance.EnableInputField(false); // ���� ���� ������ �Է� �ʵ带 ��Ȱ��ȭ�մϴ�.
        UpdateReadyButtonText();
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

    [PunRPC]
    public void SubmitGuess(string guess, int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string result = CheckGuess(guess); // ������ Ȯ���Ͽ� ��� ���
            photonView.RPC("ReceiveResult", RpcTarget.All, playerId, guess, result); // ��� Ŭ���̾�Ʈ���� ��� ����
        }
    }

    /// <summary>
    /// ////////////////////���� ���� �� �غ�
    /// </summary>
    public void PlayerReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            readyPlayersCount++;
            UpdateReadyButtonText();
            CheckAllPlayersReady();
        }
    }

    public void PlayerNotReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            readyPlayersCount = Mathf.Max(0, readyPlayersCount - 1);
            UpdateReadyButtonText();
        }
    }

    private void CheckAllPlayersReady()
    {
        // �� ���� �÷��̾� ���� �����ϰ� ��� �÷��̾ �غ�� ��쿡�� ���� ����
        if (teamManager.Team1.Count == teamManager.Team2.Count && !isGameStarting)
        {
            photonView.RPC("StartGameCountdownRPC", RpcTarget.All);
        }
        else
        {
            photonView.RPC("ShowUnbalancedTeamsMessageRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartGameCountdownRPC()
    {
        StartCoroutine(StartGameCountdown());
    }

    private IEnumerator StartGameCountdown()
    {
        isGameStarting = true;
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        countdownText.text = "Go!";
        yield return new WaitForSeconds(1);
        countdownText.text = "";
        photonView.RPC("GameStart", RpcTarget.All);
        isGameStarting = false;
    }

    [PunRPC]
    private void ShowUnbalancedTeamsMessageRPC()
    {
        StartCoroutine(ShowUnbalancedTeamsMessage());
    }

    private IEnumerator ShowUnbalancedTeamsMessage()
    {
        countdownText.text = "�ο����� ���� �ʽ��ϴ�";
        yield return new WaitForSeconds(1);
        countdownText.text = "";
    }

    [PunRPC]
    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateSecretNumber();
        }
        UIManager.Instance.EnableInputField(true); // ���� ���� �� �Է� �ʵ带 Ȱ��ȭ�մϴ�.
    }

    public void OnReadyButtonClicked() //�Ƹ� �Ⱦ���
    {
        photonView.RPC("TogglePlayerReadyState", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void TogglePlayerReadyState(Photon.Realtime.Player player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (playerManager.ReadyPlayers.Contains(player))
            {
                playerManager.ReadyPlayers.Remove(player);
                PlayerNotReady();
            }
            else
            {
                playerManager.ReadyPlayers.Add(player);
                PlayerReady();
            }
        }
    }

    public void UpdateReadyButtonText()
    {
        if (playerManager.ReadyPlayers.Contains(PhotonNetwork.LocalPlayer))
        {
            readyButton.GetComponentInChildren<Text>().text = $"{readyPlayersCount}/{playerManager.RoomPlayerList.Count}";
        }
        else
        {
            readyButton.GetComponentInChildren<Text>().text = "Ready";
        }
    }

}

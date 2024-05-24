using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Text GameLog;
    private string secretNumber; // ��� ��ȣ�� �����ϴ� ����
    private List<string> guesses = new List<string>(); // �÷��̾��� ������ �����ϴ� ����Ʈ

    public PlayerManager playerManager; // playerManager�� ����
    public TeamManager teamManager; //TeamManager�� ����
    public ChatManager chatManager; //ChatManager�� ����

    private bool isGameStarting = false;
    public Text countdownText; //ī��Ʈ�ٿ� �ؽ�Ʈ
    public Button readyButton; //���� �����غ� ��ư

    void Start()
    {
        UIManager.Instance.EnableInputField(false); // ���� ���� ������ �Է� �ʵ带 ��Ȱ��ȭ
        UpdateStartGameButtonVisibility(); //���常 ��ư�� ���̵��� ����
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
            CheckAllPlayersReady();
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
        UpdateStartGameButtonVisibility();
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        countdownText.text = "Go!";
        yield return new WaitForSeconds(1);
        countdownText.text = "";
        photonView.RPC("GameStart", RpcTarget.All);
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
    // ���常 ��ư�� ���̵��� ����
    public void UpdateStartGameButtonVisibility()
    {
        readyButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && !isGameStarting);
    }

    public void GameEnd()
    {
        isGameStarting = false;
        UIManager.Instance.EnableInputField(false); // ���� ���� �� �Է� �ʵ带 ��Ȱ��ȭ
        UpdateStartGameButtonVisibility(); // ���� ���� �� ��ư �ٽ� ���̱�
    }

}

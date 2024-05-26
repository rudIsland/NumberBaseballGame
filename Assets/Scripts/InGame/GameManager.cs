using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using Photon.Pun.Demo.PunBasics;

public class GameManager : MonoBehaviourPunCallbacks//, IPunObservable
{
    public Text GameLog;
    private string secretNumber; // ��� ��ȣ�� �����ϴ� ����
    private string secretNumber2; // ��� ��ȣ�� �����ϴ� ����
    private List<string> guesses = new List<string>(); // �÷��̾��� ������ �����ϴ� ����Ʈ



    public PlayerManager playerManager; // playerManager�� ����
    public TeamManager teamManager; //TeamManager�� ����
    public ChatManager chatManager; //ChatManager�� ����
    public UIManager UIManager;

    private bool isGameStarting = false;
    public Text countdownText; //ī��Ʈ�ٿ� �ؽ�Ʈ
    public Button readyButton; //���� �����غ� ��ư


    public GameObject InputNumberPanel; //�ʱ� ���� �Է��ǳ�
    public InputField inputNumberField;
    public Button InputNumberBtn; //�ʱ� ���� �Է¹�ư
    public bool IsTeam1Turn { get; private set; } = true; // �� 1�� �� ����

    private bool isTeam1NumberSet = false;
    private bool isTeam2NumberSet = false;

    public Text ExText;

    void Start()
    {
        ResetGame(); // ���� ���� �� �ʱ� ���·� ����
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
            photonView.RPC("EXPrint", RpcTarget.All);
        }
        else
        {
            photonView.RPC("ShowUnbalancedTeamsMessageRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ShowUnbalancedTeamsMessageRPC()
    {
        StartCoroutine(ShowUnbalancedTeamsMessage());
    }

    private IEnumerator ShowUnbalancedTeamsMessage() //�ο����� �ȸ������
    {
        UIManager.errorText.text = "�ο����� ���� �ʽ��ϴ�";
        yield return new WaitForSeconds(1);
        UIManager.errorText.text = "";
    }

    [PunRPC]
    public void IsInputNumberPanelActiveRPC()
    {
        // ��� �÷��̾�� �ǳ��� ����
        InputNumberPanel.SetActive(false);

        // �� 1�� �� 2�� ù ��° �÷��̾�� �ǳ��� ���̵��� ����
        if (PhotonNetwork.LocalPlayer == teamManager.Team1[0] || PhotonNetwork.LocalPlayer == teamManager.Team2[0])
        {
            InputNumberPanel.SetActive(true);
        }
    }

    [PunRPC]
    public IEnumerator EXPrint()
    {
        ExText.text = "���� �׵θ��� ���� �ǹ��մϴ�";
        yield return new WaitForSeconds(2);

        ExText.text = "�� ���� 1���� ���ڸ� �Է��� �� ������";
        yield return new WaitForSeconds(2);

        ExText.text = "�� �� ���� ���ڸ� ���մϴ�.";
        yield return new WaitForSeconds(2);


        ExText.text = "";
        photonView.RPC("IsInputNumberPanelActiveRPC", RpcTarget.All);
    }

    public void InputNumber()
    {
        if (UIManager.Instance.IsValidGuess(inputNumberField.text)) // �Է¹޾ƿ� ���� �ùٸ� ���
        {
            if (PhotonNetwork.LocalPlayer == teamManager.Team1[0]) // �� 1�� ù ��° �÷��̾��� ���
            {
                secretNumber = inputNumberField.text; // ��� ���� ����
                isTeam1NumberSet = true; // �� 1 ��� ���� ���� �Ϸ�
                Debug.Log("Team 1 ���� ����: " + secretNumber); // ����� �α� �߰�
                photonView.RPC("SetSecretNumber", RpcTarget.All, secretNumber, 1);
            }
            if (PhotonNetwork.LocalPlayer == teamManager.Team2[0]) // �� 2�� ù ��° �÷��̾��� ���
            {
                secretNumber2 = inputNumberField.text; // ��� ���� ����
                isTeam2NumberSet = true; // �� 2 ��� ���� ���� �Ϸ�
                Debug.Log("Team 2 ���� ����: " + secretNumber2); // ����� �α� �߰�
                photonView.RPC("SetSecretNumber", RpcTarget.All, secretNumber2, 2);
            }
            InputNumberPanel.SetActive(false); // �Է� �ǳ� ��Ȱ��ȭ
        }
    }

    [PunRPC]
    private void SetSecretNumber(string number, int team)
    {
        if (team == 1)
        {
            secretNumber = number;
            Debug.Log("Team 1 ��� ���� ���� �Ϸ�: " + secretNumber);
            isTeam1NumberSet = true;
            photonView.RPC("SetTeam1NumberSet", RpcTarget.MasterClient);
        }
        else if (team == 2)
        {
            secretNumber2 = number;
            Debug.Log("Team 2 ��� ���� ���� �Ϸ�: " + secretNumber2);
            isTeam2NumberSet = true;
            photonView.RPC("SetTeam2NumberSet", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void SetTeam1NumberSet()
    {
        isTeam1NumberSet = true;
        StartCountdownIfReady();
    }

    [PunRPC]
    private void SetTeam2NumberSet()
    {
        isTeam2NumberSet = true;
        StartCountdownIfReady();
    }

    private void StartCountdownIfReady()
    {
        if (isTeam1NumberSet && isTeam2NumberSet)
        {
            photonView.RPC("StartGameCountdownRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartGameCountdownRPC()
    {
        StartCoroutine(StartGameCountdown());
    }

    private IEnumerator StartGameCountdown()
    {
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
    public void GameStart() // ���� ����
    {
        Debug.Log($"Secret Number 1: {secretNumber}");
        Debug.Log($"Secret Number 2: {secretNumber2}");
        readyButton.gameObject.SetActive(false); // ���� ���� �� readyButton ��Ȱ��ȭ
        isGameStarting = true;
        UIManager.Instance.EnableInputPanelsForTeam(true, IsTeam1Turn); // ���� ���� �� �� 1�� �Է� �ǳ��� Ȱ��ȭ

        // ���� ���� �� ��� Ŭ���̾�Ʈ�� �� 1�� ������ ǥ��
        PhotonView UIphotonView = UIManager.GetComponent<PhotonView>();
        UIphotonView.RPC("DisplayTurnUI", RpcTarget.All, true); // �� 1�� ������ ����
    }


    // ���常 ��ư�� ���̵��� ����
    public void UpdateStartGameButtonVisibility()
    {
        readyButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && !isGameStarting);
    }

    [PunRPC]
    public void GameEnd()
    {
        isGameStarting = false; // ���� ���� ���¸� false�� ����
        photonView.RPC("ResetGameRPC", RpcTarget.All); // ��� Ŭ���̾�Ʈ���� ResetGame ȣ��
    }


    [PunRPC]
    public void SubmitGuess(string guess, int playerActorNumber, PhotonMessageInfo info)
    {
        bool isTeam1 = teamManager.Team1.Exists(player => player.ActorNumber == playerActorNumber);
        string result = CheckGuess(guess, isTeam1); // ������ Ȯ���Ͽ� ��� ���
        photonView.RPC("ReceiveResult", RpcTarget.All, info.Sender.NickName, guess, result, isTeam1); // ��� Ŭ���̾�Ʈ���� ��� ����

        if (result == "win")
        {
            StartCoroutine(IsWin(isTeam1));
        }
    }

    [PunRPC]
    public string CheckGuess(string guess, bool isTeam1)
    {
        string targetNumber = isTeam1 ? secretNumber2 : secretNumber; // targetNumber �ʱ�ȭ ����
        int strikes = 0; // ��Ʈ����ũ ��
        int balls = 0; // �� ��

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == targetNumber[i]) // ���� ��ġ�� ���� ���ڰ� �ִ� ���
            {
                strikes++;
            }
            else if (targetNumber.Contains(guess[i].ToString())) // �ٸ� ��ġ�� ���� ���ڰ� �ִ� ���
            {
                balls++;
            }
        }

        guesses.Add(guess); // ������ ����Ʈ�� �߰�
        if (strikes == 4)
        {
            return "win";
        }
        return $"{strikes}S {balls}B"; // ��� ��ȯ
    }

    [PunRPC]
    public IEnumerator IsWin(bool isTeam1)
    {
        PhotonView UIphotonView = UIManager.GetComponent<PhotonView>();
        if (isTeam1)
        {
            photonView.RPC("SetCountdownText", RpcTarget.All, "Team1 Win");
            photonView.RPC("SetWinningTeamPanelBorder", RpcTarget.All, 1);
        }
        else
        {
            photonView.RPC("SetCountdownText", RpcTarget.All, "Team2 Win");
            photonView.RPC("SetWinningTeamPanelBorder", RpcTarget.All, 2);
        }
        yield return new WaitForSeconds(2);
        photonView.RPC("SetCountdownText", RpcTarget.All, "");

        UIphotonView.RPC("ResetWinningTeamPanel", RpcTarget.All);
        photonView.RPC("GameEnd", RpcTarget.All); // ��� Ŭ���̾�Ʈ���� ResetGame ȣ��
    }

    [PunRPC]
    public void SetCountdownText(string text)
    {
        countdownText.text = text;
    }


    [PunRPC]
    public void SetWinningTeamPanelBorder(int winningTeam)
    {
        UIManager.Instance.SetWinningTeamUI(winningTeam);
    }


    [PunRPC]
    void ReceiveResult(string senderName, string guess, string result, bool isTeam1)
    {
        Debug.Log($"{senderName} guessed {guess}, Result: {result}");
        UIManager.Instance.DisplayResult($"{guess}: {result}");
        PhotonView UIphotonView = UIManager.GetComponent<PhotonView>();

        if (result != "win")
        {
            if (isTeam1)
            {
                IsTeam1Turn = false;
                Debug.Log("Switching to Team 2's turn");
            }
            else
            {
                IsTeam1Turn = true;
                Debug.Log("Switching to Team 1's turn");
            }

            UIphotonView.RPC("DisplayTurnUI", RpcTarget.All, IsTeam1Turn);
            UIManager.Instance.EnableInputPanelsForTeam(true, IsTeam1Turn);
        }
        else
        {
            UIManager.Instance.DisplayResult($"{4}S: {0}B");
        }
    }

    [PunRPC]
    public void ResetGameRPC()
    {
        ResetGame();
    }

    [PunRPC]
    private void ResetGame()
    {
        secretNumber = "";
        secretNumber2 = "";
        guesses.Clear();
        isTeam1NumberSet = false;
        isTeam2NumberSet = false;
        IsTeam1Turn = true;
        UIManager.Instance.EnableInputPanels(false);// ���� ���� ������ �Է� �ǳ��� ��Ȱ��ȭ
        InputNumberPanel.SetActive(false); //���� ���� ������ ���� �Է� �ǳ��� ��Ȱ��ȭ
        countdownText.text = "";
        readyButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
 
        UpdateStartGameButtonVisibility();// ���常 ��ư�� ���̵��� ����
        PhotonView UIphotonView = UIManager.GetComponent<PhotonView>();
        UIphotonView.RPC("UIReset", RpcTarget.All); // ��� Ŭ���̾�Ʈ���� �Է� �г��� ��Ȱ��ȭ
    }
}


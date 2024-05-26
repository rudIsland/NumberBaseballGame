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
    private string secretNumber; // 비밀 번호를 저장하는 변수
    private string secretNumber2; // 비밀 번호를 저장하는 변수
    private List<string> guesses = new List<string>(); // 플레이어의 추측을 저장하는 리스트



    public PlayerManager playerManager; // playerManager를 연결
    public TeamManager teamManager; //TeamManager를 연결
    public ChatManager chatManager; //ChatManager를 연결
    public UIManager UIManager;

    private bool isGameStarting = false;
    public Text countdownText; //카운트다운 텍스트
    public Button readyButton; //게임 시작준비 버튼


    public GameObject InputNumberPanel; //초기 숫자 입력판넬
    public InputField inputNumberField;
    public Button InputNumberBtn; //초기 숫자 입력버튼
    public bool IsTeam1Turn { get; private set; } = true; // 팀 1의 턴 여부

    private bool isTeam1NumberSet = false;
    private bool isTeam2NumberSet = false;

    public Text ExText;

    void Start()
    {
        ResetGame(); // 게임 시작 전 초기 상태로 설정
    }

    

    /// <summary>
    /// ////////////////////게임 시작 전 준비
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
        // 각 팀의 플레이어 수가 동일하고 모든 플레이어가 준비된 경우에만 게임 시작
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

    private IEnumerator ShowUnbalancedTeamsMessage() //인원수가 안맞을경우
    {
        UIManager.errorText.text = "인원수가 맞지 않습니다";
        yield return new WaitForSeconds(1);
        UIManager.errorText.text = "";
    }

    [PunRPC]
    public void IsInputNumberPanelActiveRPC()
    {
        // 모든 플레이어에게 판넬을 숨김
        InputNumberPanel.SetActive(false);

        // 팀 1과 팀 2의 첫 번째 플레이어에게 판넬을 보이도록 설정
        if (PhotonNetwork.LocalPlayer == teamManager.Team1[0] || PhotonNetwork.LocalPlayer == teamManager.Team2[0])
        {
            InputNumberPanel.SetActive(true);
        }
    }

    [PunRPC]
    public IEnumerator EXPrint()
    {
        ExText.text = "빨간 테두리는 턴을 의미합니다";
        yield return new WaitForSeconds(2);

        ExText.text = "각 팀의 1번만 숫자를 입력할 수 있으며";
        yield return new WaitForSeconds(2);

        ExText.text = "곧 각 팀의 숫자를 정합니다.";
        yield return new WaitForSeconds(2);


        ExText.text = "";
        photonView.RPC("IsInputNumberPanelActiveRPC", RpcTarget.All);
    }

    public void InputNumber()
    {
        if (UIManager.Instance.IsValidGuess(inputNumberField.text)) // 입력받아온 값이 올바른 경우
        {
            if (PhotonNetwork.LocalPlayer == teamManager.Team1[0]) // 팀 1의 첫 번째 플레이어일 경우
            {
                secretNumber = inputNumberField.text; // 비밀 숫자 설정
                isTeam1NumberSet = true; // 팀 1 비밀 숫자 설정 완료
                Debug.Log("Team 1 설정 숫자: " + secretNumber); // 디버깅 로그 추가
                photonView.RPC("SetSecretNumber", RpcTarget.All, secretNumber, 1);
            }
            if (PhotonNetwork.LocalPlayer == teamManager.Team2[0]) // 팀 2의 첫 번째 플레이어일 경우
            {
                secretNumber2 = inputNumberField.text; // 비밀 숫자 설정
                isTeam2NumberSet = true; // 팀 2 비밀 숫자 설정 완료
                Debug.Log("Team 2 설정 숫자: " + secretNumber2); // 디버깅 로그 추가
                photonView.RPC("SetSecretNumber", RpcTarget.All, secretNumber2, 2);
            }
            InputNumberPanel.SetActive(false); // 입력 판넬 비활성화
        }
    }

    [PunRPC]
    private void SetSecretNumber(string number, int team)
    {
        if (team == 1)
        {
            secretNumber = number;
            Debug.Log("Team 1 비밀 숫자 설정 완료: " + secretNumber);
            isTeam1NumberSet = true;
            photonView.RPC("SetTeam1NumberSet", RpcTarget.MasterClient);
        }
        else if (team == 2)
        {
            secretNumber2 = number;
            Debug.Log("Team 2 비밀 숫자 설정 완료: " + secretNumber2);
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
    public void GameStart() // 게임 시작
    {
        Debug.Log($"Secret Number 1: {secretNumber}");
        Debug.Log($"Secret Number 2: {secretNumber2}");
        readyButton.gameObject.SetActive(false); // 게임 시작 시 readyButton 비활성화
        isGameStarting = true;
        UIManager.Instance.EnableInputPanelsForTeam(true, IsTeam1Turn); // 게임 시작 시 팀 1의 입력 판넬을 활성화

        // 게임 시작 시 모든 클라이언트에 팀 1의 턴임을 표시
        PhotonView UIphotonView = UIManager.GetComponent<PhotonView>();
        UIphotonView.RPC("DisplayTurnUI", RpcTarget.All, true); // 팀 1의 턴으로 설정
    }


    // 방장만 버튼이 보이도록 설정
    public void UpdateStartGameButtonVisibility()
    {
        readyButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && !isGameStarting);
    }

    [PunRPC]
    public void GameEnd()
    {
        isGameStarting = false; // 게임 시작 상태를 false로 설정
        photonView.RPC("ResetGameRPC", RpcTarget.All); // 모든 클라이언트에서 ResetGame 호출
    }


    [PunRPC]
    public void SubmitGuess(string guess, int playerActorNumber, PhotonMessageInfo info)
    {
        bool isTeam1 = teamManager.Team1.Exists(player => player.ActorNumber == playerActorNumber);
        string result = CheckGuess(guess, isTeam1); // 추측을 확인하여 결과 얻기
        photonView.RPC("ReceiveResult", RpcTarget.All, info.Sender.NickName, guess, result, isTeam1); // 모든 클라이언트에게 결과 전송

        if (result == "win")
        {
            StartCoroutine(IsWin(isTeam1));
        }
    }

    [PunRPC]
    public string CheckGuess(string guess, bool isTeam1)
    {
        string targetNumber = isTeam1 ? secretNumber2 : secretNumber; // targetNumber 초기화 수정
        int strikes = 0; // 스트라이크 수
        int balls = 0; // 볼 수

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == targetNumber[i]) // 같은 위치에 같은 숫자가 있는 경우
            {
                strikes++;
            }
            else if (targetNumber.Contains(guess[i].ToString())) // 다른 위치에 같은 숫자가 있는 경우
            {
                balls++;
            }
        }

        guesses.Add(guess); // 추측을 리스트에 추가
        if (strikes == 4)
        {
            return "win";
        }
        return $"{strikes}S {balls}B"; // 결과 반환
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
        photonView.RPC("GameEnd", RpcTarget.All); // 모든 클라이언트에서 ResetGame 호출
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
        UIManager.Instance.EnableInputPanels(false);// 게임 시작 전에는 입력 판넬을 비활성화
        InputNumberPanel.SetActive(false); //게임 시작 전에는 숫자 입력 판넬을 비활성화
        countdownText.text = "";
        readyButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
 
        UpdateStartGameButtonVisibility();// 방장만 버튼이 보이도록 설정
        PhotonView UIphotonView = UIManager.GetComponent<PhotonView>();
        UIphotonView.RPC("UIReset", RpcTarget.All); // 모든 클라이언트에서 입력 패널을 비활성화
    }
}


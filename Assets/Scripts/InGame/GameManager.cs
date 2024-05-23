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
    private string secretNumber; // 비밀 번호를 저장하는 변수
    private List<string> guesses = new List<string>(); // 플레이어의 추측을 저장하는 리스트

    public PlayerManager playerManager; // playerManager를 연결
    public TeamManager teamManager; //TeamManager를 연결

    private bool isGameStarting = false;
    private int readyPlayersCount = 0;
    public Text countdownText; //카운트다운 텍스트
    public Button readyButton;

    void Start()
    {
        UIManager.Instance.EnableInputField(false); // 게임 시작 전에는 입력 필드를 비활성화합니다.
        UpdateReadyButtonText();
    }

    void GenerateSecretNumber()
    {
        // 0-9 사이의 중복되지 않는 4자리 숫자를 생성
        List<int> digits = new List<int>();
        while (digits.Count < 4)
        {
            int randomDigit = Random.Range(0, 10);
            if (!digits.Contains(randomDigit))
            {
                digits.Add(randomDigit);
            }
        }
        secretNumber = string.Join("", digits); // 리스트를 문자열로 변환하여 비밀 번호 설정
        Debug.Log(secretNumber);
    }

    public string CheckGuess(string guess)
    {
        int strikes = 0; // 스트라이크 수
        int balls = 0; // 볼 수

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == secretNumber[i]) // 같은 위치에 같은 숫자가 있는 경우
            {
                strikes++;
            }
            else if (secretNumber.Contains(guess[i].ToString())) // 다른 위치에 같은 숫자가 있는 경우
            {
                balls++;
            }
        }

        guesses.Add(guess); // 추측을 리스트에 추가
        return $"{strikes}S {balls}B"; // 결과 반환
    }

    [PunRPC]
    public void SubmitGuess(string guess, PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string result = CheckGuess(guess); // 추측을 확인하여 결과 얻기
            photonView.RPC("ReceiveResult", RpcTarget.All, info.Sender.NickName, guess, result); // 모든 클라이언트에게 결과 전송
        }
    }

    [PunRPC]
    void ReceiveResult(string senderName, string guess, string result)
    {
        // 결과를 UI에 표시하는 로직 (임시로 디버그 로그 사용)
        Debug.Log($"{senderName} guessed {guess}, Result: {result}");
        // UI 매니저의 DisplayResult 메서드를 호출하여 결과를 표시
        UIManager.Instance.DisplayResult($"{guess}: {result}");
    }

    [PunRPC]
    public void SubmitGuess(string guess, int playerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string result = CheckGuess(guess); // 추측을 확인하여 결과 얻기
            photonView.RPC("ReceiveResult", RpcTarget.All, playerId, guess, result); // 모든 클라이언트에게 결과 전송
        }
    }

    /// <summary>
    /// ////////////////////게임 시작 전 준비
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
        // 각 팀의 플레이어 수가 동일하고 모든 플레이어가 준비된 경우에만 게임 시작
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
        countdownText.text = "인원수가 맞지 않습니다";
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
        UIManager.Instance.EnableInputField(true); // 게임 시작 시 입력 필드를 활성화합니다.
    }

    public void OnReadyButtonClicked() //아마 안쓸듯
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

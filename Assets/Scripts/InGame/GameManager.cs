using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public Text GameLog;
    private string secretNumber; // 비밀 번호를 저장하는 변수
    private List<string> guesses = new List<string>(); // 플레이어의 추측을 저장하는 리스트

    public PlayerManager playerManager; // playerManager를 연결
    public TeamManager teamManager; //TeamManager를 연결
    public ChatManager chatManager; //ChatManager를 연결

    private bool isGameStarting = false;
    public Text countdownText; //카운트다운 텍스트
    public Button readyButton; //게임 시작준비 버튼

    /*타이머 및 시간 관련 함수*/
    public Slider progressBar; // 프로그래스 바
    public Button submitGuessButton; // 정답 제출 버튼
    public Button startGameButton; // 게임 시작 버튼
    public Text timerText; // 남은 시간 텍스트 표시
                           // 
    private float gameDuration = 10f; // 게임 시간(1분)
    private float answerDuration = 10f; // 정답 입력 시간(10초)
    private bool isAnswerTime = false;
    private bool isTimerRunning = false;
    private float elapsedTime = 0f;
    private float currentDuration = 0f;
    private float progress = 0f;
    private string currentPhase = "게임 시간";

    void Start()
    {
        UIManager.Instance.EnableInputField(false); // 게임 시작 전에는 입력 필드를 비활성화
        UpdateStartGameButtonVisibility(); //방장만 버튼이 보이도록 설정
        startGameButton.onClick.AddListener(OnStartGame);
        submitGuessButton.interactable = false;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= currentDuration)
            {
                elapsedTime = currentDuration;
                isTimerRunning = false;
            }
            UpdateUI();
        }
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
    // 방장만 버튼이 보이도록 설정
    public void UpdateStartGameButtonVisibility()
    {
        readyButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && !isGameStarting);
    }

    public void GameEnd()
    {
        isGameStarting = false;
        UIManager.Instance.EnableInputField(false); // 게임 종료 시 입력 필드를 비활성화
        UpdateStartGameButtonVisibility(); // 게임 종료 시 버튼 다시 보이기
    }

    /* 타이머 
     * + 
     * 프로그래스바 게임 시간확인*/

    private void UpdateUI()
    {
        progress = elapsedTime / currentDuration;
        progressBar.value = progress;
        int remainingTime = Mathf.CeilToInt(currentDuration - elapsedTime);
        timerText.text = $"{currentPhase}: {remainingTime}초 남음";
    }

    private IEnumerator GameTimer()
    {
        while (true)
        {
            yield return StartCoroutine(RunTimer(gameDuration, "게임 시간"));
            isAnswerTime = true;
            photonView.RPC("SetSubmitButtonInteractable", RpcTarget.All, true);
            yield return StartCoroutine(RunTimer(answerDuration, "정답 입력 시간"));
            isAnswerTime = false;

        }
    }

    private IEnumerator RunTimer(float duration, string phase)
    {
        elapsedTime = 0f;
        currentDuration = duration;
        currentPhase = phase;
        isTimerRunning = true;

        while (isTimerRunning)
        {
            yield return null;
        }

        timerText.text = $"{phase} 종료";
    }
    [PunRPC]
    private void OnSubmitGuess()
    {
        if (isAnswerTime)
        {
            photonView.RPC("ResetTimer", RpcTarget.All);
        }
    }

    private void OnStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartTimer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartTimer()
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;
            StartCoroutine(GameTimer());
        }
    }

    [PunRPC]
    private void ResetTimer()
    {
        StopAllCoroutines();
        isTimerRunning = false;
        StartCoroutine(GameTimer());
    }

    [PunRPC]
    private void SetSubmitButtonInteractable(bool interactable)
    {
        submitGuessButton.interactable = interactable;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터 전송: 마스터 클라이언트가 타이머 정보를 전송
            stream.SendNext(elapsedTime);
            stream.SendNext(currentDuration);
            stream.SendNext(currentPhase);
            stream.SendNext(isTimerRunning);
            stream.SendNext(submitGuessButton.interactable);
            stream.SendNext(progress);
        }
        else
        {
            try
            {

                // 데이터 수신: 클라이언트가 타이머 정보를 수신
                elapsedTime = (float)stream.ReceiveNext();
                currentDuration = (float)stream.ReceiveNext();
                currentPhase = (string)stream.ReceiveNext();
                isTimerRunning = (bool)stream.ReceiveNext();
                progress = (float)stream.ReceiveNext();

                bool isSubmitButtonInteractable = (bool)stream.ReceiveNext();

                // 수신된 정보를 바탕으로 타이머 상태 업데이트
                submitGuessButton.interactable = isSubmitButtonInteractable;
                UpdateUI();

            }
            catch { }

        }
    }

}

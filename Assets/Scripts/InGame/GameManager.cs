using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
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

    /*Ÿ�̸� �� �ð� ���� �Լ�*/
    public Slider progressBar; // ���α׷��� ��
    public Button submitGuessButton; // ���� ���� ��ư
    public Button startGameButton; // ���� ���� ��ư
    public Text timerText; // ���� �ð� �ؽ�Ʈ ǥ��
                           // 
    private float gameDuration = 10f; // ���� �ð�(1��)
    private float answerDuration = 10f; // ���� �Է� �ð�(10��)
    private bool isAnswerTime = false;
    private bool isTimerRunning = false;
    private float elapsedTime = 0f;
    private float currentDuration = 0f;
    private float progress = 0f;
    private string currentPhase = "���� �ð�";

    void Start()
    {
        UIManager.Instance.EnableInputField(false); // ���� ���� ������ �Է� �ʵ带 ��Ȱ��ȭ
        UpdateStartGameButtonVisibility(); //���常 ��ư�� ���̵��� ����
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

    /* Ÿ�̸� 
     * + 
     * ���α׷����� ���� �ð�Ȯ��*/

    private void UpdateUI()
    {
        progress = elapsedTime / currentDuration;
        progressBar.value = progress;
        int remainingTime = Mathf.CeilToInt(currentDuration - elapsedTime);
        timerText.text = $"{currentPhase}: {remainingTime}�� ����";
    }

    private IEnumerator GameTimer()
    {
        while (true)
        {
            yield return StartCoroutine(RunTimer(gameDuration, "���� �ð�"));
            isAnswerTime = true;
            photonView.RPC("SetSubmitButtonInteractable", RpcTarget.All, true);
            yield return StartCoroutine(RunTimer(answerDuration, "���� �Է� �ð�"));
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

        timerText.text = $"{phase} ����";
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
            // ������ ����: ������ Ŭ���̾�Ʈ�� Ÿ�̸� ������ ����
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

                // ������ ����: Ŭ���̾�Ʈ�� Ÿ�̸� ������ ����
                elapsedTime = (float)stream.ReceiveNext();
                currentDuration = (float)stream.ReceiveNext();
                currentPhase = (string)stream.ReceiveNext();
                isTimerRunning = (bool)stream.ReceiveNext();
                progress = (float)stream.ReceiveNext();

                bool isSubmitButtonInteractable = (bool)stream.ReceiveNext();

                // ���ŵ� ������ �������� Ÿ�̸� ���� ������Ʈ
                submitGuessButton.interactable = isSubmitButtonInteractable;
                UpdateUI();

            }
            catch { }

        }
    }

}

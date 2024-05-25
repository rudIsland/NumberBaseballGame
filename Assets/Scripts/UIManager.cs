using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance; // 싱글톤 인스턴스

    public InputField Team1_guessInput; // 추측 입력 필드
    public InputField Team2_guessInput; // 추측 입력 필드
    public GameObject Team1_Input_GuessPanel; // 팀 1 입력 판넬
    public GameObject Team2_Input_GuessPanel; // 팀 2 입력 판넬


    public Text resultText; // 결과를 표시하는 텍스트
    public GameManager gameManager; // 게임 매니저 참조
    public TeamManager teamManager;
    public Text errorText; // ErrorText UI 요소

    void Awake()
    {
        // UIManager 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스가 생성되지 않도록 합니다.
        }
    }

    private void Start()
    {
        EnableInputPanels(false); // 게임 시작 전에는 입력 필드를 비활성화합니다.
    }

    [PunRPC]
    public void OnSubmitGuess()
    {
        string guess = gameManager.IsTeam1Turn ? Team1_guessInput.text : Team2_guessInput.text; // 입력된 추측 가져오기
        if (IsValidGuess(guess)) // 간단한 입력 검증 및 중복 숫자 검사
        {
            //우리팀한테만 보내게 수정해야함.
            PhotonView photonView = gameManager.GetComponent<PhotonView>();
            photonView.RPC("SubmitGuess", RpcTarget.MasterClient, guess, PhotonNetwork.LocalPlayer.ActorNumber); // 마스터 클라이언트에게 추측 전송
            //photonView.RPC("OnSubmitGuess", RpcTarget.All);
            //photonView.RPC("SetSubmitButtonInteractable", RpcTarget.All, false);
        }
    }

    public bool IsValidGuess(string guess)
    {
        if (guess.Length != 4)
        {
            DisplayError("입력은 4자리 숫자여야 합니다."); // 에러 메시지 출력
            return false;
        }

        HashSet<char> uniqueChars = new HashSet<char>();
        foreach (char c in guess)
        {
            if (!char.IsDigit(c))
            {
                DisplayError("입력은 숫자만 포함해야 합니다."); // 에러 메시지 출력
                return false;
            }
            if (!uniqueChars.Add(c))
            {
                DisplayError("입력은 중복된 숫자를 포함할 수 없습니다."); // 에러 메시지 출력
                return false;
            }
        }

        return true;
    }

    private void DisplayError(string message)
    {
        // 에러 메시지를 화면에 표시
        if (errorText != null)
        {
            errorText.text = message;
        }
        else
        {
            Debug.LogWarning("Error text component is not assigned.");
        }
    }

    public void DisplayResult(string message)
    {
        resultText.text = "\n" + message; // 결과를 텍스트 UI에 추가하여 표시
        errorText.text = "";
    }

    public void EnableInputPanels(bool enable)
    {
        Team1_Input_GuessPanel.SetActive(enable && gameManager.IsTeam1Turn);
        Team2_Input_GuessPanel.SetActive(enable && !gameManager.IsTeam1Turn);
    }

    public void EnableInputPanelsForTeam(bool enable, bool isTeam1)
    {
        if (isTeam1)
        {
            Team1_Input_GuessPanel.SetActive(enable && teamManager.Team1.Contains(PhotonNetwork.LocalPlayer));
            Team2_Input_GuessPanel.SetActive(false);
        }
        else
        {
            Team1_Input_GuessPanel.SetActive(false);
            Team2_Input_GuessPanel.SetActive(enable && teamManager.Team2.Contains(PhotonNetwork.LocalPlayer));
        }
    }

}
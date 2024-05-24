using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // 싱글톤 인스턴스

    public InputField guessInput; // 추측 입력 필드
    public Text resultText; // 결과를 표시하는 텍스트
    public GameManager gameManager; // 게임 매니저 참조
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
        EnableInputField(false); // 게임 시작 전에는 입력 필드를 비활성화합니다.
    }


    public void OnSubmitGuess()
    {
        string guess = guessInput.text; // 입력된 추측 가져오기
        if (IsValidGuess(guess)) // 간단한 입력 검증 및 중복 숫자 검사
        {
            PhotonView photonView = gameManager.GetComponent<PhotonView>();
            photonView.RPC("SubmitGuess", RpcTarget.MasterClient, guess); // 마스터 클라이언트에게 추측 전송
            photonView.RPC("OnSubmitGuess", RpcTarget.All);
            photonView.RPC("SetSubmitButtonInteractable", RpcTarget.All, false);
        }
    }

    private bool IsValidGuess(string guess)
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


    public void EnableInputField(bool enable)
    {
        guessInput.interactable = enable; // 입력 필드 활성화 또는 비활성화
        if (!enable)
        {
            guessInput.text = ""; // 비활성화 시 입력 필드를 비웁니다.
        }
    }
}
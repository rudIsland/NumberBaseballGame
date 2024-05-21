using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // 싱글톤 인스턴스

    public InputField guessInput; // 추측 입력 필드
    public Text resultText; // 결과를 표시하는 텍스트
    public GameManager gameManager; // 게임 매니저 참조

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

    public void OnSubmitGuess()
    {
        string guess = guessInput.text; // 입력된 추측 가져오기
        if (guess.Length == 4) // 간단한 입력 검증
        {
            PhotonView photonView = gameManager.GetComponent<PhotonView>();
            photonView.RPC("SubmitGuess", RpcTarget.MasterClient, guess); // 마스터 클라이언트에게 추측 전송
        }
    }

    public void DisplayResult(string message)
    {
        resultText.text += "\n" + message; // 결과를 텍스트 UI에 추가하여 표시
    }
}
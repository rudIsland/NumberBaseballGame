using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance; // 싱글톤 인스턴스

    public InputField Team1_guessInput; // 추측 입력 필드
    public InputField Team2_guessInput; // 추측 입력 필드
    public GameObject Team1_Input_GuessPanel; // 팀 1 입력 판넬
    public GameObject Team2_Input_GuessPanel; // 팀 2 입력 판넬


    public GameObject Team1TurnPanel; //팀1턴일때 표시할 판넬
    public GameObject Team2TurnPanel; //팀2턴일때 표시할 판넬
    private Image team1Image;
    private Image team2Image;


    public Text resultText; // 결과를 표시하는 텍스트
    public GameManager gameManager; // 게임 매니저 참조
    public TeamManager teamManager;
    public Text errorText; // ErrorText UI 요소

    private List<string> team1Guess = new List<string>(); // 팀 1 추측 리스트
    private List<string> team2Guess = new List<string>(); // 팀 2 추측 리스트

    public GameObject Team1GuessPanel; //팀1 정답리스트 판넬
    public GameObject Team2GuessPanel; //팀2 정답리스트 판넬

    public Transform  Team1ContentPanel; // Team1 스크롤뷰의 Content Panel
    public Transform  Team2ContentPanel; // Team2 스크롤뷰의 Content Panel

    public GameObject Team1GuessePrefab; // 채팅 메시지 프리팹
    public GameObject Team2GuessePrefab; // 채팅 메시지 프리팹

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
        Team1GuessPanel.SetActive(false); 
        Team2GuessPanel.SetActive(false);
        team1Image = Team1TurnPanel.GetComponent<Image>(); // 투명도를 위해 가져옴
        team2Image = Team2TurnPanel.GetComponent<Image>(); // 투명도를 위해 가져옴

        EnableInputPanels(false); // 게임 시작 전에는 입력 필드를 비활성화합니다.
        // 패널의 Image 컴포넌트 가져오기
        UIReset();
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
        }
    }

    public bool IsValidGuess(string guess)
    {
        if (guess.Length != 4)
        {
            StartCoroutine(DisplayErrorCoroutine("입력은 4자리 숫자여야 합니다.")); // 에러 메시지 출력
            return false;
        }

        HashSet<char> uniqueChars = new HashSet<char>();
        foreach (char c in guess)
        {
            if (!char.IsDigit(c))
            {
                StartCoroutine(DisplayErrorCoroutine("입력은 숫자만 포함해야 합니다.")); // 에러 메시지 출력
                return false;
            }
            if (!uniqueChars.Add(c))
            {
                StartCoroutine(DisplayErrorCoroutine("입력은 중복된 숫자를 포함할 수 없습니다.")); // 에러 메시지 출력("입력은 중복된 숫자를 포함할 수 없습니다."); // 에러 메시지 출력
                return false;
            }
        }

        return true;
    }

    private IEnumerator DisplayErrorCoroutine(string message)
    {
        // 에러 메시지를 화면에 표시
        if (errorText != null)
        {
            errorText.text = message;
            yield return new WaitForSeconds(1);
            errorText.text = "";
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

    public void Inputguess(string message, bool isTeam)
    {
        if (isTeam)
        {
            team1Guess.Add(message);
            UpdateTeam1Display(team1Guess);
        }
        else
        {
            team2Guess.Add(message);
            UpdateTeam2Display(team2Guess);
        }   
    }

    private void UpdateTeam1Display(List<string> messagesToDisplay)
    {
        // 기존 메시지 모두 제거
        foreach (Transform child in Team1ContentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (string message in messagesToDisplay)
        {
            GameObject newMessage = Instantiate(Team1GuessePrefab, Team1ContentPanel);
            Text messageText = newMessage.GetComponent<Text>();
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        Canvas.ForceUpdateCanvases(); // 캔버스 강제 업데이트
    }

    private void UpdateTeam2Display(List<string> messagesToDisplay)
    {
        // 기존 메시지 모두 제거
        foreach (Transform child in Team2ContentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (string message in messagesToDisplay)
        {
            GameObject newMessage = Instantiate(Team2GuessePrefab, Team2ContentPanel);
            Text messageText = newMessage.GetComponent<Text>();
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        Canvas.ForceUpdateCanvases(); // 캔버스 강제 업데이트
    }


    public void EnableInputPanels(bool enable)
    {
        Team1_Input_GuessPanel.SetActive(enable && gameManager.IsTeam1Turn);
        Team2_Input_GuessPanel.SetActive(enable && !gameManager.IsTeam1Turn);
    }

    [PunRPC]
    public void UIReset()
    {
        Team1_Input_GuessPanel.SetActive(false);
        Team2_Input_GuessPanel.SetActive(false);
        Team1_guessInput.text = "";
        Team2_guessInput.text = "";
        resultText.text = "";
        errorText.text = "";
        SetPanelTransparency(team1Image, 0f);
        SetPanelTransparency(team2Image, 0f);
        SetPanelBorderColor(team1Image, Color.red);
        SetPanelBorderColor(team2Image, Color.red);
        team1Guess.Clear();
        team2Guess.Clear();
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


    [PunRPC]
    public void DisplayTurnUI(bool isTeam1)
    {
        Debug.Log($"DisplayTurnUI 호출됨: isTeam1 = {isTeam1}"); // 디버그 로그 추가
        if (isTeam1)
        {
            SetPanelTransparency(team1Image, 1f);
            Debug.Log("Team1TurnPanel 투명도를 1f로 설정");
            SetPanelTransparency(team2Image, 0f);
            Debug.Log("Team2TurnPanel 투명도를 0f로 설정");
        }
        else
        {
            SetPanelTransparency(team1Image, 0f);
            Debug.Log("Team1TurnPanel 투명도를 0f로 설정");
            SetPanelTransparency(team2Image, 1f);
            Debug.Log("Team2TurnPanel 투명도를 1f로 설정");
        }
    }

    public void SetPanelTransparency(Image image, float alpha)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
            Debug.Log($"{image.gameObject.name}의 투명도를 {alpha}로 설정"); // 디버그 로그 추가
        }
    }
    public void SetPanelBorderColor(Image image, Color borderColor)
    {
        if (image != null)
        {
            image.color = borderColor;
        }
    }



    [PunRPC]
    public void SetWinningTeamUI(int winningTeam)
    {
        SetPanelTransparency(team1Image, 1f);
        SetPanelTransparency(team2Image, 1f);

        if (winningTeam == 1)
        {
            SetPanelBorderColor(team2Image, Color.black);
            SetPanelBorderColor(team1Image, Color.green);
            Debug.Log("Team1TurnPanel을 green으로 설정하고 Team2TurnPanel을 black으로 설정");
        }
        else
        {
            SetPanelBorderColor(team1Image, Color.black);
            SetPanelBorderColor(team2Image, Color.green);
            Debug.Log("Team2TurnPanel을 green으로 설정하고 Team1TurnPanel을 black으로 설정");
        }
    }

    [PunRPC]
    public void ResetWinningTeamPanel()
    {
        SetPanelTransparency(team1Image, 1f);
        SetPanelTransparency(team2Image, 1f);

        SetPanelBorderColor(team1Image, Color.red);
        SetPanelBorderColor(team2Image, Color.red);

        Debug.Log("모든 패널의 투명도를 1f로 설정하고 색상을 red로 설정");
    }

    public void Team1GuessListExit()
    {
        Team1GuessPanel.SetActive(false);
    }
    public void Team2GuessListExit()
    {
        Team2GuessPanel.SetActive(false);
    }
    public void Team1GuessListOpen()
    {
        Team1GuessPanel.SetActive(true); 
    }
    public void Team2GuessListOpen()
    {
        Team2GuessPanel.SetActive(true);
    }
}
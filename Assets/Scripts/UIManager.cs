using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance; // �̱��� �ν��Ͻ�

    public InputField Team1_guessInput; // ���� �Է� �ʵ�
    public InputField Team2_guessInput; // ���� �Է� �ʵ�
    public GameObject Team1_Input_GuessPanel; // �� 1 �Է� �ǳ�
    public GameObject Team2_Input_GuessPanel; // �� 2 �Է� �ǳ�


    public GameObject Team1TurnPanel; //��1���϶� ǥ���� �ǳ�
    public GameObject Team2TurnPanel; //��2���϶� ǥ���� �ǳ�
    private Image team1Image;
    private Image team2Image;


    public Text resultText; // ����� ǥ���ϴ� �ؽ�Ʈ
    public GameManager gameManager; // ���� �Ŵ��� ����
    public TeamManager teamManager;
    public Text errorText; // ErrorText UI ���

    void Awake()
    {
        // UIManager �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ��� �������� �ʵ��� �մϴ�.
        }
    }

    private void Start()
    {
        team1Image = Team1TurnPanel.GetComponent<Image>(); // ������ ���� ������
        team2Image = Team2TurnPanel.GetComponent<Image>(); // ������ ���� ������

        EnableInputPanels(false); // ���� ���� ������ �Է� �ʵ带 ��Ȱ��ȭ�մϴ�.
        // �г��� Image ������Ʈ ��������
        UIReset();
    }

    [PunRPC]
    public void OnSubmitGuess()
    {
        string guess = gameManager.IsTeam1Turn ? Team1_guessInput.text : Team2_guessInput.text; // �Էµ� ���� ��������
        if (IsValidGuess(guess)) // ������ �Է� ���� �� �ߺ� ���� �˻�
        {
            //�츮�����׸� ������ �����ؾ���.
            PhotonView photonView = gameManager.GetComponent<PhotonView>();
            photonView.RPC("SubmitGuess", RpcTarget.MasterClient, guess, PhotonNetwork.LocalPlayer.ActorNumber); // ������ Ŭ���̾�Ʈ���� ���� ����
        }
    }

    public bool IsValidGuess(string guess)
    {
        if (guess.Length != 4)
        {
            StartCoroutine(DisplayErrorCoroutine("�Է��� 4�ڸ� ���ڿ��� �մϴ�.")); // ���� �޽��� ���
            return false;
        }

        HashSet<char> uniqueChars = new HashSet<char>();
        foreach (char c in guess)
        {
            if (!char.IsDigit(c))
            {
                StartCoroutine(DisplayErrorCoroutine("�Է��� ���ڸ� �����ؾ� �մϴ�.")); // ���� �޽��� ���
                return false;
            }
            if (!uniqueChars.Add(c))
            {
                StartCoroutine(DisplayErrorCoroutine("�Է��� �ߺ��� ���ڸ� ������ �� �����ϴ�.")); // ���� �޽��� ���("�Է��� �ߺ��� ���ڸ� ������ �� �����ϴ�."); // ���� �޽��� ���
                return false;
            }
        }

        return true;
    }

    private IEnumerator DisplayErrorCoroutine(string message)
    {
        // ���� �޽����� ȭ�鿡 ǥ��
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
        resultText.text = "\n" + message; // ����� �ؽ�Ʈ UI�� �߰��Ͽ� ǥ��
        errorText.text = "";
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
        Debug.Log($"DisplayTurnUI ȣ���: isTeam1 = {isTeam1}"); // ����� �α� �߰�
        if (isTeam1)
        {
            SetPanelTransparency(team1Image, 1f);
            Debug.Log("Team1TurnPanel ������ 1f�� ����");
            SetPanelTransparency(team2Image, 0f);
            Debug.Log("Team2TurnPanel ������ 0f�� ����");
        }
        else
        {
            SetPanelTransparency(team1Image, 0f);
            Debug.Log("Team1TurnPanel ������ 0f�� ����");
            SetPanelTransparency(team2Image, 1f);
            Debug.Log("Team2TurnPanel ������ 1f�� ����");
        }
    }

    public void SetPanelTransparency(Image image, float alpha)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
            Debug.Log($"{image.gameObject.name}�� ������ {alpha}�� ����"); // ����� �α� �߰�
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
            Debug.Log("Team1TurnPanel�� green���� �����ϰ� Team2TurnPanel�� black���� ����");
        }
        else
        {
            SetPanelBorderColor(team1Image, Color.black);
            SetPanelBorderColor(team2Image, Color.green);
            Debug.Log("Team2TurnPanel�� green���� �����ϰ� Team1TurnPanel�� black���� ����");
        }
    }

    [PunRPC]
    public void ResetWinningTeamPanel()
    {
        SetPanelTransparency(team1Image, 1f);
        SetPanelTransparency(team2Image, 1f);

        SetPanelBorderColor(team1Image, Color.red);
        SetPanelBorderColor(team2Image, Color.red);

        Debug.Log("��� �г��� ������ 1f�� �����ϰ� ������ red�� ����");
    }
}
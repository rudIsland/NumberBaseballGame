using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance; // �̱��� �ν��Ͻ�

    public InputField Team1_guessInput; // ���� �Է� �ʵ�
    public InputField Team2_guessInput; // ���� �Է� �ʵ�
    public GameObject Team1_Input_GuessPanel; // �� 1 �Է� �ǳ�
    public GameObject Team2_Input_GuessPanel; // �� 2 �Է� �ǳ�


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
        EnableInputPanels(false); // ���� ���� ������ �Է� �ʵ带 ��Ȱ��ȭ�մϴ�.
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
            //photonView.RPC("OnSubmitGuess", RpcTarget.All);
            //photonView.RPC("SetSubmitButtonInteractable", RpcTarget.All, false);
        }
    }

    public bool IsValidGuess(string guess)
    {
        if (guess.Length != 4)
        {
            DisplayError("�Է��� 4�ڸ� ���ڿ��� �մϴ�."); // ���� �޽��� ���
            return false;
        }

        HashSet<char> uniqueChars = new HashSet<char>();
        foreach (char c in guess)
        {
            if (!char.IsDigit(c))
            {
                DisplayError("�Է��� ���ڸ� �����ؾ� �մϴ�."); // ���� �޽��� ���
                return false;
            }
            if (!uniqueChars.Add(c))
            {
                DisplayError("�Է��� �ߺ��� ���ڸ� ������ �� �����ϴ�."); // ���� �޽��� ���
                return false;
            }
        }

        return true;
    }

    private void DisplayError(string message)
    {
        // ���� �޽����� ȭ�鿡 ǥ��
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
        resultText.text = "\n" + message; // ����� �ؽ�Ʈ UI�� �߰��Ͽ� ǥ��
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
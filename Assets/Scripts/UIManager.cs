using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // �̱��� �ν��Ͻ�

    public InputField guessInput; // ���� �Է� �ʵ�
    public Text resultText; // ����� ǥ���ϴ� �ؽ�Ʈ
    public GameManager gameManager; // ���� �Ŵ��� ����
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
        EnableInputField(false); // ���� ���� ������ �Է� �ʵ带 ��Ȱ��ȭ�մϴ�.
    }


    public void OnSubmitGuess()
    {
        string guess = guessInput.text; // �Էµ� ���� ��������
        if (IsValidGuess(guess)) // ������ �Է� ���� �� �ߺ� ���� �˻�
        {
            PhotonView photonView = gameManager.GetComponent<PhotonView>();
            photonView.RPC("SubmitGuess", RpcTarget.MasterClient, guess); // ������ Ŭ���̾�Ʈ���� ���� ����
            photonView.RPC("OnSubmitGuess", RpcTarget.All);
            photonView.RPC("SetSubmitButtonInteractable", RpcTarget.All, false);
        }
    }

    private bool IsValidGuess(string guess)
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


    public void EnableInputField(bool enable)
    {
        guessInput.interactable = enable; // �Է� �ʵ� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
        if (!enable)
        {
            guessInput.text = ""; // ��Ȱ��ȭ �� �Է� �ʵ带 ���ϴ�.
        }
    }
}
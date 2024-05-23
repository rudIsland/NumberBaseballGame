using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // �̱��� �ν��Ͻ�

    public InputField guessInput; // ���� �Է� �ʵ�
    public Text resultText; // ����� ǥ���ϴ� �ؽ�Ʈ
    public GameManager gameManager; // ���� �Ŵ��� ����

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
        if (guess.Length == 4) // ������ �Է� ����
        {
            PhotonView photonView = gameManager.GetComponent<PhotonView>();
            photonView.RPC("SubmitGuess", RpcTarget.MasterClient, guess); // ������ Ŭ���̾�Ʈ���� ���� ����
        }
    }

    public void DisplayResult(string message)
    {
        resultText.text += "\n" + message; // ����� �ؽ�Ʈ UI�� �߰��Ͽ� ǥ��
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
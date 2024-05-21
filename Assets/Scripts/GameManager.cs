using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{

    public Text GameLog;
    private string secretNumber; // 비밀 번호를 저장하는 변수
    private List<string> guesses = new List<string>(); // 플레이어의 추측을 저장하는 리스트

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateSecretNumber(); // 비밀 번호 생성
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

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entered the room.");
        GameLog.text += newPlayer.NickName + " entered the room.\n";
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " left the room.");
        GameLog.text += otherPlayer.NickName + " left the room.";
    }
}

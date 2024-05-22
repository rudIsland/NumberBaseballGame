using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class dont : MonoBehaviour
{
    private static dont instance;

    // �ı��� ���� �̸�
    public string sceneToDestroy = "GameRoom";
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void OnEnable()
    {
        // �� ��ȯ�� ������ �̺�Ʈ ������ ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // �� ��ȯ�� ������ �̺�Ʈ ������ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ư�� ������ ��ȯ�Ǿ��� �� ������Ʈ�� �ı�
        if (scene.name == sceneToDestroy)
        {
            Destroy(gameObject);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class dont : MonoBehaviour
{
    private static dont instance;

    // 파괴될 씬의 이름
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
        // 씬 전환을 감지할 이벤트 리스너 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 씬 전환을 감지할 이벤트 리스너 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 특정 씬으로 전환되었을 때 오브젝트를 파괴
        if (scene.name == sceneToDestroy)
        {
            Destroy(gameObject);
        }
    }
}
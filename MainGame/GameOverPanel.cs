using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button returnToLobbyBtn;
    [SerializeField] private GameObject chlidObj;
    
    private void Start()
    {
        GameManager.OnGameIsOver += OnMatchIsOver;
        // returnToLobbyBtn.onClick.AddListener(() => GlobalManagers.Instance.networkRunnerController.ShutDownRunner());
        returnToLobbyBtn.onClick.AddListener(() => GlobalManagers.Instance.networkRunnerController.ShutDownRunner());
    }

    private void OnMatchIsOver()
    {
        chlidObj.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.OnGameIsOver -= OnMatchIsOver;
    }
}

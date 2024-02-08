using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionEntryPrefab : MonoBehaviour
{
    public TextMeshProUGUI sessionName, playerCount;
    public Button joinButton;

    private void Awake()
    {
        joinButton.onClick.AddListener(JoinSession);
    }

    private void Start()
    {
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }

    private void JoinSession()
    {
        GlobalManagers.Instance.networkRunnerController.JoinSession(sessionName.text);
    }
}

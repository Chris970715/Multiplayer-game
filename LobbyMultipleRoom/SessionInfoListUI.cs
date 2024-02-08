using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Unity.VisualScripting;

public class SessionInfoListUI : MonoBehaviour
{
    public static SessionInfoListUI Instance;
    
    public TextMeshProUGUI sessionNameText;
    public TextMeshProUGUI playerCountText;
    public Button joinButton;

    private SessionInfo sessionInfo;
    
    // Session info contains all details about the session, such as name of the session, the player count, and the details
    //  we want to have

    public event Action<SessionInfo> OnJoinSession;

    private void Awake()
    {
        Instance = this;
    }

    public void SetInformation(SessionInfo sessionInfo)
    {
        this.sessionInfo = sessionInfo;
        sessionNameText.text = sessionInfo.Name;
        playerCountText.text = $"{sessionInfo.PlayerCount.ToString()} / {sessionInfo.MaxPlayers.ToString()}";

        bool isJoinButtonActive = true;

        if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
        {
            isJoinButtonActive = false;
        }
        else
        {
            joinButton.gameObject.SetActive(true);
        }
    }

    public void Onclick()
    {
        OnJoinSession?.Invoke(sessionInfo);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNickNamePanel : LobbyPanelBase
{
    [Header("CreateNickNamePanel Variables (Derived Class)")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button createNickNameBtn;
    [SerializeField] private Button tutorialBtn;
    
    private bool IsFromDiscord = false;
    private bool IsHost = false;
    private string discordName = "Test";

    private const int MIN_CHAR_FOR_NICKNAME = 2;
    
    public override void InitPanel(LobbyUIManager lobbyUIManager)
    {
        //Calling InitPanel from Base Class
        base.InitPanel(lobbyUIManager);
        // Not allowing users to leave empty name
        createNickNameBtn.interactable = false;
        createNickNameBtn.onClick.AddListener(OnclickCreateNickName);
        tutorialBtn.onClick.AddListener(OpenTutorialPanel);
        inputField.onValueChanged.AddListener(onInputValueChanged);
        
        FromDiscordEvent();
    }

    private void onInputValueChanged(string arg0)
    {
        // Making the button to be clickable when user types atleast two chars
        createNickNameBtn.interactable = arg0.Length >= MIN_CHAR_FOR_NICKNAME;
    }

    private async void OnclickCreateNickName()
    {
        var nickName = inputField.text;

        if (nickName.Length >= MIN_CHAR_FOR_NICKNAME)
        {
            AudioManager.Instance.PlayButtonClip();
            await GlobalManagers.Instance.networkRunnerController.ConnectToLobby(nickName);
            
            base.ClosePanel();
            // Requesting UI manager to disable the first panel(Nickname Panel)
            lobbyUIManager.ShowPanel(LobbyPanelType.LobbyPanel);
            // wanting to wait the animation clip length and only then disable the panel
            
        }
    }

    private void FromDiscordEvent()
    {
        if (IsFromDiscord)
        {
            AudioManager.Instance.PlayButtonClip();
            GlobalManagers.Instance.networkRunnerController.SetPlayerNickname(discordName);
            base.ClosePanel();
            
            if (IsHost)
            {
                lobbyUIManager.ShowPanel(LobbyPanelType.DiscordHostPanel);
            }
            else
            {
                lobbyUIManager.ShowPanel(LobbyPanelType.DiscordClientPanel);
            }
        }
    }

    private void OpenTutorialPanel()
    {
        AudioManager.Instance.PlayButtonClip();
        lobbyUIManager.ShowPanel(LobbyPanelType.TutorialPanel);
    }
    
    
}

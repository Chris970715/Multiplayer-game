using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomPanel : LobbyPanelBase
{
    [Header("JoinRoomPanel Variables")]

    [SerializeField]
    private Button closeJoinRoomBtn;

    [SerializeField]
    private TMP_InputField jonRoomInputField;

    [SerializeField]
    private GameObject createJoinRoomPanel;

    [SerializeField]
    private Button joinRoomBtn;

    private NetworkRunnerController networkRunnerController;

    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);

        // Not to call everytime I need GlovalManager Instance
        networkRunnerController = GlobalManagers.Instance.networkRunnerController;
        //joinRandomBtn.onClick.AddListener(joinRandomRoom);

        // Inside the button container panel
        joinRoomBtn.onClick.AddListener(() => JoinRoom(jonRoomInputField.text));

        // close button container panel
        closeJoinRoomBtn.onClick.AddListener(() => closeJoinRoomPanel());
    }

    /*
    // These turn on/off button container panel
    private void openCreateRoomPanel()
    {
        createJoinRoomPanel.SetActive(true);
    }
    */
    private void closeJoinRoomPanel()
    {
        AudioManager.Instance.PlayButtonClip();
        base.ClosePanel();
        lobbyUIManager.ShowPanel(LobbyPanelType.LobbyPanel);
    }

    private void JoinRoom(string roomCode)
    {   
        if (roomCode.Length >= 2)
        {
            AudioManager.Instance.PlayButtonClip();
            // Start game function in network controller
            networkRunnerController.JoinSession(roomCode);
        }
    }

    private void joinRandomRoom()
    {
        AudioManager.Instance.PlayButtonClip();
        Debug.Log($"--------------JoinRandomRoom---------------");
        // networkRunnerController.StartGame(GameMode.AutoHostOrClient, String.Empty);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiddleSectionPanel : LobbyPanelBase
{
    [Header("MiddleSectionPanel Variables")]
    //[SerializeField] private Button joinRandomBtn;
    //[SerializeField] private Button openCreateRoomBtn;
    [SerializeField]
    private Button closeCreateRoomBtn;

    //[SerializeField] private TMP_InputField joinRoomByArgInputField;
    [SerializeField]
    private TMP_InputField creatRoomInputField;

    [SerializeField]
    public TMP_Dropdown timeOptions;

    [SerializeField]
    private GameObject createJoinRoomPanel;

    // Inside the button container
    [SerializeField]
    private Button createRoomBtn;

    [SerializeField]
    private Button joinRoomBtn;

    public float gameTime;

    private NetworkRunnerController networkRunnerController;

    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);

        // Not to call everytime I need GlovalManager Instance
        networkRunnerController = GlobalManagers.Instance.networkRunnerController;
        //joinRandomBtn.onClick.AddListener(joinRandomRoom);

        // Inside the button container panel
        //joinRoomBtn.onClick.AddListener(() => CreateRoom(joinRoomByArgInputField.text));
        createRoomBtn.onClick.AddListener(() => CreateRoom(creatRoomInputField.text, timeOptions.value));

        // Open and close button container panel
        //openCreateRoomBtn.onClick.AddListener(() => openCreateRoomPanel());
        closeCreateRoomBtn.onClick.AddListener(() => closeCreateRoomPanel());
    }

    /*
    // These turn on/off button container panel
    private void openCreateRoomPanel()
    {
        createJoinRoomPanel.SetActive(true);
    }
    */
    private void closeCreateRoomPanel()
    {
        AudioManager.Instance.PlayButtonClip();
        base.ClosePanel();
        lobbyUIManager.ShowPanel(LobbyPanelType.LobbyPanel);
    }

    private void CreateRoom(string field, int optionIndex)
    {
        // Assign float value from the option index
        switch (optionIndex)
        {
            // 5 minutes
            case 0:
                gameTime = 300f;
                break;
            // 10 minutes
            case 1:
                gameTime = 600f;
                break;
            // 15 minutes
            case 2:
                gameTime = 900f;
                break;
        }
        
        if (field.Length >= 2 && gameTime > 0)
        {
            AudioManager.Instance.PlayButtonClip();
            // Start game function in network controller
            networkRunnerController.StartGame(field, gameTime);
        }
    }

    private void joinRandomRoom()
    {
        AudioManager.Instance.PlayButtonClip();
        Debug.Log($"--------------JoinRandomRoom---------------");
        // networkRunnerController.StartGame(GameMode.AutoHostOrClient, String.Empty);
    }
}

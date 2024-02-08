using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class DiscordClient : LobbyPanelBase
{
    [Header("Discord Host Variables")]
    [SerializeField] private Button joinRoomBtn;
    
    private NetworkRunnerController networkRunnerController;
    private string roomName = "Null1";

    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);
        networkRunnerController = GlobalManagers.Instance.networkRunnerController;
        joinRoomBtn.onClick.AddListener(() => CreateRoom(GameMode.Client, roomName));
    }
    
    private void CreateRoom(GameMode mode, string field)
    {
        if (field.Length >= 2)
        {
            AudioManager.Instance.PlayButtonClip();
            // create a room
            Debug.Log($"--------------{mode}---------------");
            networkRunnerController.StartGame(field, 300f);
        }
    }
}

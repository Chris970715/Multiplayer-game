using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : LobbyPanelBase
{
    [SerializeField] private Button openCreateRoomBtn;
    [SerializeField] private Button openJoinRoomBtn;
    [SerializeField] private Button characterSelectButton;
    private WebGLManager webGLManager;

    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);
        webGLManager = FindObjectOfType<WebGLManager>();
        openCreateRoomBtn.onClick.AddListener(() => openCreateRoomPanel());
        openJoinRoomBtn.onClick.AddListener(() => openJoinRoomPanel());
        characterSelectButton.onClick.AddListener(() => webGLManager.HandleOpenCharacterDialog());
    }

    private void openCreateRoomPanel()
    {
        AudioManager.Instance.PlayButtonClip();
        base.ClosePanel();
        lobbyUIManager.ShowPanel(LobbyPanelType.MiddleSectionPanel);
    }

    private void openJoinRoomPanel()
    {
        AudioManager.Instance.PlayButtonClip();
        base.ClosePanel();
        lobbyUIManager.ShowPanel(LobbyPanelType.JoinRoomPanel);
    }

    
    
}

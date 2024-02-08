using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    // This Manager is to determine whether to show panel or not

    [SerializeField] private LobbyPanelBase[] lobbyPanels;
    

    // To instantiate loading Canvas prefab
    [SerializeField] private LoadingCavasController loadingCavasControllerPrefab;
    private void Start()
    {
        foreach (var lobby in lobbyPanels)
        {
            lobby.InitPanel(this);
        }

        Instantiate(loadingCavasControllerPrefab);
    }

    public void ShowPanel(LobbyPanelBase.LobbyPanelType type)
    {
        foreach (var lobby in lobbyPanels)
        {
            if (lobby.PanelType == type)
            {
                // Since Nickname Panel's layer is lower than Manu, the NickName Panel disappear.
                lobby.ShowPanel();        
                break;
            }
        }
    }
}

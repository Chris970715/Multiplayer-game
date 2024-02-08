using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanelBase : MonoBehaviour
{
    [field: SerializeField, Header("LobbyPanel Variables(Base Class)")]
    [SerializeField] public LobbyPanelType PanelType { get; private set; }
    [SerializeField] private Animator panelAnimator;
    
    protected LobbyUIManager lobbyUIManager;
    
    // To distinguish children of LobbyPanelBase
    public enum LobbyPanelType
    {
        None,
        CreateNickNamePanel,
        MiddleSectionPanel,
        JoinRoomPanel,
        DiscordHostPanel,
        DiscordClientPanel,
        LobbyPanel,
        TutorialPanel
    }

    public virtual void InitPanel(LobbyUIManager uiManager)
    {
        lobbyUIManager = uiManager;
    }

    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
        const string POP_IN_CLIP_NAME = "In";
        panelAnimator.Play(POP_IN_CLIP_NAME);
    }

    protected void ClosePanel()
    {
        const string POP_OUT_CLIP_NAME = "Out";
        // The following static method is to prevent nickNamePanel being disabled even after user goes back to the nicjName 
        // from the second manu ㄱ.
        StartCoroutine(Utils.PlayAnimationAndSetStateWhenFinished(gameObject, panelAnimator, POP_OUT_CLIP_NAME, false));
    }
}

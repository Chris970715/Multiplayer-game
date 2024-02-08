using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using System;

public class PlayerNameSetter : NetworkBehaviour
{
    public event Action<string> OnNickNameSet;
    [SerializeField] private TextMeshProUGUI[] playerNameTexts;
    
    [Networked(OnChanged = nameof(OnNickNameChanged)), HideInInspector]
    public NetworkString<_16> NickName { get; private set;}

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            var nickName = GlobalManagers.Instance.networkRunnerController.LocalPlayerNickName;
            RpcSetNickName(nickName);
        }
    }
    
    //We wrap it under an RPC AND an OnChanged as we want it to be updated to late joiners 
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RpcSetNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }
    
    public static void OnNickNameChanged(Changed<PlayerNameSetter> changed)
    {
        changed.Behaviour.SetPlayerNicknameText(changed.Behaviour.NickName);
    }
    
    private void SetPlayerNicknameText(NetworkString<_16> nickName)
    {
        foreach (var playerNameText in playerNameTexts)
        {
            if (playerNameText.gameObject.activeSelf)
                playerNameText.text = nickName.Value;
        }

        OnNickNameSet?.Invoke(nickName.Value);
    }
}

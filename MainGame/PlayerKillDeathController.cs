using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerKillDeathController : NetworkBehaviour
{
    public Dictionary<int, PlayerLeaderboardDataMono> CurrentPlayers { get; private set; } = new();
    [Networked(OnChanged = nameof(KillChanged))] public int CurrentKillAmount { get; set;}
    [Networked(OnChanged = nameof(DeathChanged))] public int CurrentDeathAmount { get; set; }
    
    
    

    [Rpc(sources: RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_IncreasePlayerKillAmount()
    {
        CurrentKillAmount++;
    }
    
    [Rpc(sources: RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_IncreasePlayerDeathAmount()
    {
        CurrentDeathAmount++;
    }
    
    public override void Spawned()
    {
        CurrentKillAmount = 0;
        CurrentDeathAmount = 0;
    }
    

    private static void KillChanged(Changed<PlayerKillDeathController> chagned)
    {
        var currentKillAmount = chagned.Behaviour.CurrentKillAmount;
        
        chagned.LoadOld();

        var oldKillAmount = chagned.Behaviour.CurrentKillAmount;

        if (currentKillAmount != oldKillAmount)
        {
            // Call method to increase kill amounts.
            
        }
    }
    
    private static void DeathChanged(Changed<PlayerKillDeathController> chagned)
    {
        var currentDeathAmount = chagned.Behaviour.CurrentDeathAmount;
        
        chagned.LoadOld();

        var oldDeathAmount = chagned.Behaviour.CurrentKillAmount;

        if (currentDeathAmount != oldDeathAmount)
        {
            // Call method to increase Death amounts.
            

        }
    }

    private void UpdateVisualKillAmount()
    {
        
    }
    
    private void UpdateVisualDeathAmount()
    {
        
    }
}

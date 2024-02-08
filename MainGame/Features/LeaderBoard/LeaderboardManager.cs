using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;

public class LeaderboardManager : NetworkBehaviour
{
    
    public Dictionary<int, PlayerLeaderboardDataMono> CurrentPlayers { get; private set;} = new();

    // the Capacity represents the number of players that it can print.
    [Networked(OnChanged = nameof(OnListChanged)), Capacity(16)] // Sep-30-2023 - Chris -> Capacity represents a max num of players in the game
                                                                       // This has to be same as the max number in Contoller
    private NetworkLinkedList<int> currentPlayerInLeaderBoard => default;

    private GlobalManagers globalManagers;

    private void Awake()
    {
        globalManagers = GlobalManagers.Instance;

        if (globalManagers != null)
        {
            globalManagers.LeaderboardManager = this;
        }

        globalManagers.networkRunnerController.OnPlayerLeftRoom += OnPlayerLeftRoom;
    }

    //If a player leaves a room, we want to remove him from our UI (hence we remove it from our lists)
    private void OnPlayerLeftRoom(NetworkRunner runner, PlayerRef playerRef)
    {
        if (currentPlayerInLeaderBoard.Contains(playerRef.PlayerId))
        {
            currentPlayerInLeaderBoard.Remove(playerRef.PlayerId);
        }

        if (CurrentPlayers.ContainsKey(playerRef.PlayerId))
        {
            CurrentPlayers.Remove(playerRef.PlayerId);
        }
    }

    //Once Fusion detected a change (adding or removing a player) 
    //This function will be called with the new data of the instance already updated
    public static void OnListChanged(Changed<LeaderboardManager> changed)
    {
        changed.Behaviour.CompareAndUpdateList(changed.Behaviour.currentPlayerInLeaderBoard);
    }

    private void CompareAndUpdateList(NetworkLinkedList<int> list)
    {
        if (list.Count > 0)
        {
            foreach (var item in list.Where(item => !CurrentPlayers.ContainsKey(item)))
            {
                globalManagers.playerSpawnerController.CurrentSpawnedPlayers.TryGetValue(item, out var player);
                if (player != null)
                {
                    var leaderboardMono = player.GetComponent<PlayerLeaderboardDataMono>();
                    CurrentPlayers.Add(item, leaderboardMono);
                }
            }
        }
    }

    public void UpdatePlayersScores(NetworkObject diedPlayer, NetworkObject killerPlayer)
    {
        void Increase(NetworkObject obj, int deathIncrease, int killIncrease)
        {
            if (obj == null) return;
            obj.GetComponent<PlayerLeaderboardDataMono>().IncreaseDeathsAndKills(deathIncrease, killIncrease);
        }
        
        Increase(diedPlayer,1, 0);
        Rpc_AddKillCount(killerPlayer, 0 ,1);
        //Increase(killerPlayer,0, 1);
    }

    [Rpc(RpcSources.All, targets: RpcTargets.All)]
    public void Rpc_AddKillCount(NetworkObject obj, int deathIncrease, int killIncrease)
    {
        if (obj == null) return;
        obj.GetComponent<PlayerLeaderboardDataMono>().IncreaseDeathsAndKills(deathIncrease, killIncrease);
    }
    
    //This rpc get's called AFTER a player instance nickname HAD been set
    //Only then we are adding him to our list (pay attention that it is hooked with OnChanged)
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void Rpc_AddPlayerToList(int playerID)
    {
        if (!currentPlayerInLeaderBoard.Contains(playerID))
        {
            currentPlayerInLeaderBoard.Add(playerID);
        }
    }
    
    private void OnDestroy() //
    {
        globalManagers.networkRunnerController.OnPlayerLeftRoom -= OnPlayerLeftRoom;
    }
    
}

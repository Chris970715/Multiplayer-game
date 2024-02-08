using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    public Dictionary<int, NetworkObject> CurrentSpawnedPlayers { get; private set; } =
        new Dictionary<int, NetworkObject>();

    [Header("Player Spawn Settings")]
    // [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;
    [SerializeField]
    NetworkObject playerNetworkPrefab;

    [SerializeField]
    public Transform[] spawnPoints;

    public bool isHostSpawned = false;
    public PlayerRef localPlayerRef;

    private void Awake()
    {
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.playerSpawnerController = this;
        }
    }

    // If we uses Start(), Runner object could be null. (Runner relays on fusion not on Unity )
    public override void Spawned()
    {
        foreach (var item in Runner.ActivePlayers)
        {
            PlayerJoined(item); // only spawn Host itself.
        }
    }

    /*
     * The Fusion networking framework (or whichever system you're using) will likely call the PlayerJoined method
     * when it detects that a new player has joined the session.
     */
    public void SpawnPlayer2(NetworkObject _playerPrefab, PlayerRef playerPef)
    {
        var index = playerPef % spawnPoints.Length;
        var spawnPoint = spawnPoints[index].transform.position;

        NetworkObject playerObject = Runner.Spawn(
            _playerPrefab,
            spawnPoint,
            Quaternion.identity,
            playerPef
        );
        // NetworkObject playerObject = Runner.Spawn(playerNetworkPrefab, Vector3.zero);
        Runner.SetPlayerObject(Runner.LocalPlayer, playerObject);
    }

    private void Spawnplayer(PlayerRef playerPef)
    {
        if (
            !CurrentSpawnedPlayers.ContainsKey(playerPef.PlayerId)
            && playerPef.PlayerId == Runner.LocalPlayer.PlayerId
        )
        {
            var index = playerPef % spawnPoints.Length;
            var spawnPoint = spawnPoints[index].transform.position;
            var playerObject = Runner.Spawn(
                playerNetworkPrefab,
                spawnPoint,
                Quaternion.identity,
                playerPef
            );

            Runner.SetPlayerObject(Runner.LocalPlayer, playerObject);
        }
    }

    // When a player leaves the game, the Fusion framework will trigger the PlayerLeft() emthod , passing in the refe
    //reference of the player who left
    private void DespawnPlayer(PlayerRef playerRef) // get invoked when player leaves the game
    {
        Debug.Log("despawnPlayer");
        //Despawn a player and removes it from our dic
        if (CurrentSpawnedPlayers != null && CurrentSpawnedPlayers.ContainsKey(playerRef.PlayerId))
        {
            CurrentSpawnedPlayers.TryGetValue(playerRef.PlayerId, out var networkObject);

            CurrentSpawnedPlayers.Remove(playerRef);

            if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
            {
                Runner.Despawn(playerNetworkObject);
                GlobalManagers.Instance.ObjectPoolingManager.RemoveNetworkObejctFromDic(
                    playerNetworkObject
                );
                Destroy(playerNetworkObject);

            }
        }

        if(Object.HasStateAuthority){
            GlobalManagers.Instance.WebGLManager.finishScoreUpdate();
        }
    }

    // used to instantiate player prefabs.
    public void PlayerJoined(PlayerRef player) //Player contains player's ID
    {
        Spawnplayer(player);

        if (Runner.ActivePlayers.Count() == 1)
        {
            isHostSpawned = true;
            ItemSpawnController.Instance.StartSpawningItems();
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }

    public Vector2 GetRandomSpawnPoint()
    {
        var index = Random.Range(0, spawnPoints.Length - 1);
        return spawnPoints[index].position;
    }

    public void AddToEntry(int id, NetworkObject obj)
    {
        if (!CurrentSpawnedPlayers.ContainsKey(id))
        {
            CurrentSpawnedPlayers.Add(id, obj);
        }
    }
}

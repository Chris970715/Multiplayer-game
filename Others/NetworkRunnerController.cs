using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;

public class NetworkRunnerController : MonoBehaviour, INetworkRunnerCallbacks
{
    //Loading Action (enable and disable loading Scene)
    public event Action OnStartedRunnerConnection;
    public event Action OnFinishRunnerConnection;
    public event Action OnPlayerJoinedSucessfully;
    public event Action<NetworkRunner, PlayerRef> OnPlayerLeftRoom;

    [SerializeField]
    NetworkObject playerNetworkPrefab;

    [SerializeField]
    NetworkObject itemNetworkPrefabRef;
    public NetworkRunner runner;
    public string LocalPlayerNickName { get; private set; }
    public bool itemSpawnStarted = false;
    public bool firstRefreshed = false;

    [Header("Session List")]
    public GameObject roomListCanvas;
    private List<SessionInfo> _sessions = new List<SessionInfo>();
    public GameObject sessionEntryPrefab;

    public Button refreshButton;
    public Transform sessionListContent;

    public float matchTime = 100f;

    // This class is gonna join or create a room
    // the core when it comes to actually connecting and disconnecting as well

    // INetwrokRunnerCallbacks (interface) from Fusions

    // NetworkRunner (a part of Fusion) is reference for a prefab we will have in the game
    // NetworkRunner is the core connection. this is what is gonna manage our client or server simulation
    /*
     *  The Network Runner runs the whoe simulation, the whole networking aspect of stuff happening with Network Runner.
     *  No matter what you, client or Host, are, you have to have network runner from doing the basic connection
     *  till updating any network values and so on.
     *
     *  (empty object - assgin Netwrok Runner - make object as prefab - drag and drop to the networkRunnerPrefab)
     */
    [SerializeField]
    private NetworkRunner networkRunnerPrefab;
    private NetworkRunner networkRunnerInstance;

    // Connect player to Fusion Lobby
    public async Task ConnectToLobby(string str)
    {
        OnStartedRunnerConnection?.Invoke();
        LocalPlayerNickName = str;
        if (networkRunnerInstance == null)
        {
            networkRunnerInstance = Instantiate(networkRunnerPrefab);
            // need to call all of the method below
            networkRunnerInstance.AddCallbacks(this);

            // by enablin this, player collect the input and then sending it back to the server
            networkRunnerInstance.ProvideInput = true;
        }

        var result = await networkRunnerInstance.JoinSessionLobby(SessionLobby.Shared);

        if (result.Ok)
        {
            OnFinishRunnerConnection?.Invoke();
            //Debug.Log(result);
        }
        else
        {
            OnFinishRunnerConnection?.Invoke();
            Debug.Log("Failed to start");
        }
    }

    // main method to join the server or create a room as host
    // the following method is a part of fusion API and it is async function
    public async void StartGame(string roomName, float gameTime)
    {
        // roomlistCanvas.SetActive(false);
        OnStartedRunnerConnection?.Invoke();
        //1. At the moment we start game, we want to instantiate a nerwork runner as we want to start the actual connection
        if (networkRunnerInstance == null)
        {
            networkRunnerInstance = Instantiate(networkRunnerPrefab);
            networkRunnerInstance.AddCallbacks(this);
            networkRunnerInstance.ProvideInput = true;
        }

        // Creating Game parameters
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName, // room Name
            PlayerCount = 12, // Max player Count
            SceneManager = networkRunnerInstance.GetComponent<INetworkSceneManager>(), // Fusion takes care of scene manager
            ObjectPool = networkRunnerInstance.GetComponent<ObjectPoolingManager>()
        };

        // Aftering creating the parameters, we need to tell the instance to run the session (Joining the game)
        // or creating a room

        var result = await networkRunnerInstance.StartGame(startGameArgs);

        // Checking if the connection was successfully made
        if (result.Ok)
        {
            //If the connection was made, I want to tell instance to load new scene
            const string SCENE_NAME = "MainGame";
            networkRunnerInstance.SetActiveScene(SCENE_NAME);
            matchTime = gameTime;
        }
        else
        {
            Debug.Log($"Failed to start: {result.ShutdownReason}");
        }
    }

    public async void JoinSession(string sessionName)
    {
        // roomlistCanvas.SetActive(false);
        if (networkRunnerInstance == null)
        {
            Debug.Log("vnetworkRunnerInstance in join game");
            networkRunnerInstance = Instantiate(networkRunnerPrefab);
        }
        // // need to call all of the method below
        // networkRunnerInstance.AddCallbacks(this);

        // // by enablin this, player collect the input and then sending it back to the server
        // networkRunnerInstance.ProvideInput = true;

        // Creating Game parameters
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName, // room Name
            SceneManager = networkRunnerInstance.GetComponent<INetworkSceneManager>()
        };

        // Aftering creating the parameters, we need to tell the instance to run the session (Joining the game)
        // or creating a room

        var result = await networkRunnerInstance.StartGame(startGameArgs);

        // Checking if the connection was successfully made
        if (result.Ok)
        {
            //If the connection was made, I want to tell instance to load new scene
            const string SCENE_NAME = "MainGame";
            networkRunnerInstance.SetActiveScene(SCENE_NAME);
            matchTime = 100f;
        }
        else
        {
            Debug.Log($"Failed to start: {result.ShutdownReason}");
        }
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // Debug.Log(runner.LocalPlayer.PlayerId);
        // NetworkObject playerObject = runner.Spawn(
        //     playerNetworkPrefab,
        //     Vector2.zero,
        //     Quaternion.identity,
        //     ru
        // );
        // runner.SetPlayerObject(runner.LocalPlayer, playerObject);

        // GlobalManagers.Instance.playerSpawnerController.SpawnPlayer2(playerNetworkPrefab, runner.LocalPlayer);

        // System.Random random = new System.Random();
        // int randomNumber = random.Next(1, 4);
        // var spawnPoint = GlobalManagers.Instance.playerSpawnerController.spawnPoints[randomNumber].transform.position;
        // NetworkObject playerObject = runner.Spawn(
        //     playerNetworkPrefab,
        //     spawnPoint,
        //     Quaternion.identity
        // );
        // runner.SetPlayerObject(runner.LocalPlayer, playerObject);


        // Debug.Log($"runner.ActivePlayers.Count(): {runner.ActivePlayers.Count()}");

        // if (runner.ActivePlayers.Count() == 1)
        // {
        //     itemSpawnStarted = true;
        //     ItemSpawnController.Instance.StartSpawningItems();
        // }

        // OnPlayerJoinedSucessfully?.Invoke();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        _sessions.Clear();
        _sessions = sessionList;
        RefreshSessionListUI();
    }

    public void RefreshSessionListUI()
    {
        // clears our session list ui so we dont create duplicates
        if (sessionListContent == null)
        {
            GameObject panel = GameObject.Find("LobbyPanel");
            Transform hello;
            if (panel != null)
            {
                Transform[] allChildren = panel.GetComponentsInChildren<Transform>(true); // true ensures you get inactive children as well
                foreach (Transform child in allChildren)
                {
                    if(child.Find("Session List Content") != null) {
                        hello = child;
                        sessionListContent = hello.transform.Find("Session List Content");
                        break;
                    }
                }
            }
        }

        foreach (Transform child in sessionListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (SessionInfo session in _sessions)
        {
            if (session.IsVisible)
            {
                GameObject entry = GameObject.Instantiate(sessionEntryPrefab, sessionListContent);
                SessionEntryPrefab script = entry.GetComponent<SessionEntryPrefab>();
                script.sessionName.text = session.Name;
                script.playerCount.text = session.PlayerCount + "/" + session.MaxPlayers;

                if (session.IsOpen == false || session.PlayerCount >= session.MaxPlayers)
                {
                    script.joinButton.interactable = false;
                }
                else
                {
                    script.joinButton.interactable = true;
                }
            }
        }
    }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        OnPlayerJoinedSucessfully?.Invoke();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        OnPlayerLeftRoom?.Invoke(runner, player);
        NetworkObject _player = runner.GetPlayerObject(player);
        runner.Despawn(_player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Debug.Log("OnInput");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("OnInputMissing");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");

        const string LOBBY_SCENE = "Lobby";
        SceneManager.LoadScene(LOBBY_SCENE);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("OnDisconnectedFromServer");
    }

    public void OnConnectRequest(
        NetworkRunner runner,
        NetworkRunnerCallbackArgs.ConnectRequest request,
        byte[] token
    )
    {
        Debug.Log("OnConnectRequest");
    }

    public void OnConnectFailed(
        NetworkRunner runner,
        NetAddress remoteAddress,
        NetConnectFailedReason reason
    )
    {
        Debug.Log("OnConnectFailed");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("OnUserSimulationMessage");
    }

    public void OnCustomAuthenticationResponse(
        NetworkRunner runner,
        Dictionary<string, object> data
    )
    {
        Debug.Log("OnCustomAuthenticationResponse");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("OnHostMigration");
    }

    public void OnReliableDataReceived(
        NetworkRunner runner,
        PlayerRef player,
        ArraySegment<byte> data
    )
    {
        Debug.Log("OnReliableDataReceived");
    }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void ShutDownRunner()
    {
        networkRunnerInstance.Shutdown();
    }

    public void SetPlayerNickname(string str)
    {
        // ConnectToLobby(str);
        // LocalPlayerNickName = str;
    }
}

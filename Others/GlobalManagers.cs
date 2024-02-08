using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    // Since I don't want to call every single Singletons to call, I just want to have manager to maintain all Singletons.
    public static GlobalManagers Instance { get; private set; }

    [SerializeField] private GameObject parentobj;
    [field: SerializeField] public NetworkRunnerController networkRunnerController { get; private set; }
    public PlayerSpawnerController playerSpawnerController { get; set; }
    public ObjectPoolingManager ObjectPoolingManager { get; set; }
    [field: SerializeField] public WebGLManager WebGLManager { get; set; }
    
    public KillFeedManager KillFeedManager { get; set; }
    public LeaderboardManager LeaderboardManager { get; set; }
    public GameManager GameManager { get; set; }
    
    public AudioController AudioController { get; set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Destroy
            Destroy(parentobj);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private PlayerLeaderBoardItem playerLeaderBoardItem;
    [SerializeField] private Transform contentToSpawnPrefabAt;
    [SerializeField] private GameObject childObject;

    private readonly List<PlayerLeaderBoardItem> spawnedLeaderBoardItems = new();
    private bool isOpen;

    private void Awake()
    {
        GameManager.OnGameIsOver += OnMatchTimerEnded;
    }

    private void OnMatchTimerEnded()
    {
        SetLeaderBoardUI();
        childObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
            childObject.SetActive(isOpen);

            if (isOpen)
            {
                SetLeaderBoardUI();
            }
        }
    }

    private void SetLeaderBoardUI()
    {
        if (spawnedLeaderBoardItems?.Count > 0)
        {
            foreach (var item in spawnedLeaderBoardItems)
            {
                Destroy(item.gameObject);
            }
            
            spawnedLeaderBoardItems.Clear();
        }

        var manager = GlobalManagers.Instance.LeaderboardManager;
        
        

        if (manager != null && manager.CurrentPlayers.Count > 0)
        {
            foreach (var item in manager.CurrentPlayers)
            {
                
                var obj = Instantiate(playerLeaderBoardItem, contentToSpawnPrefabAt);
                var playerNickName = Utils.GetPlayerNickname(item.Value.Object);
                obj.Set(playerNickName, item.Value.KilledAmount, item.Value.DiedAmount);
                
                spawnedLeaderBoardItems.Add(obj);
            }
        }
    }
    
    private void OnDestroy()
    {
        GameManager.OnGameIsOver -= OnMatchTimerEnded;
    }
}

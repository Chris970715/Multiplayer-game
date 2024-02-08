using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeedManager : MonoBehaviour
{
    [SerializeField] private KilledPlayerItem killedPlayerItemPrefab;
    [SerializeField] private Transform content;

    private void Awake()
    {
        GlobalManagers.Instance.KillFeedManager = this;
    }

    public void ShowKill(string killedNickname)
    {
        var obj = Instantiate(killedPlayerItemPrefab, content);
        obj.SetName(killedNickname);
    }
}

using UnityEngine;
using Fusion;

public class PlayerLeaderboardDataMono :NetworkBehaviour
{

    [Networked, HideInInspector] public int DiedAmount{ get; private set; }
    [Networked, HideInInspector] public int KilledAmount{ get; private set; }
    
    [Networked, HideInInspector] public string nickname { get; private set; }

    public void IncreaseDeathsAndKills(int deathIncrease, int killsIncrease)
    {
        this.DiedAmount += deathIncrease;
        this.KilledAmount += killsIncrease;
    }
}

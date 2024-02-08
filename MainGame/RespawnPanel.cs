using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class RespawnPanel : SimulationBehaviour
{
    // What is SimulationBehaviour?
    // It allows us to use fixed update netowrk and still be part of the whole simulation.
    // a way to still be in the loop and use functions such as fixed update network without doing any kind of a networking related stuff
    
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextMeshProUGUI respawnAmountText;
    [SerializeField] private GameObject chlidObject;
    
    public override void FixedUpdateNetwork()
    {
        if (playerController.Object.HasInputAuthority == Runner.LocalPlayer)
        {
            // running or disabling the child, depending on the respawnAmountText
            var timerIsRunning = playerController.RespawnTimer.IsRunning;
            chlidObject.SetActive(timerIsRunning);

            if (timerIsRunning && playerController.RespawnTimer.RemainingTime(Runner).HasValue)
            {
                var time = playerController.RespawnTimer.RemainingTime(Runner).Value;
                var roundInt = Mathf.RoundToInt(time);
                respawnAmountText.text = roundInt.ToString();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    
    [field: SerializeField] public Collider2D cameraBounds { get; private set;}
    public static event Action OnGameIsOver;
    public static bool MatchIsOver { get; private set;}
    [SerializeField] public Camera cam;
    [SerializeField] private TextMeshProUGUI timerText;
    // [SerializeField] private float matchTimerAmount = FindObjectOfType<MiddleSectionPanel>().GameTime; // 기존 값 지우고 시간 여기 입력 하시오
    public float matchTimerAmount = 300f;
    
    [Networked] private TickTimer matchTimer { get; set; }

    private void Awake()
    {
        if (GlobalManagers.Instance != null)
        {
            GlobalManagers.Instance.GameManager = this;
        }
    }

    public override void Spawned()
    {
        //Reset this variable
        MatchIsOver = false;
        cam.gameObject.SetActive(false);
        matchTimerAmount = GlobalManagers.Instance.networkRunnerController.matchTime;
        matchTimer = TickTimer.CreateFromSeconds(Runner, matchTimerAmount);
    }

    // [Rpc(sources: RpcSources.All, RpcTargets.StateAuthority)]
    // private void SetTimer(NetworkString<_32> nickName)
    // {
    //     matchTimer = TickTimer.CreateFromSeconds(Runner, matchTimerAmount);
    // }

    public override void FixedUpdateNetwork()
    {
        if (matchTimer.Expired(Runner) == false && matchTimer.RemainingTime(Runner).HasValue)
        {
            var timeSpan = TimeSpan.FromSeconds(matchTimer.RemainingTime(Runner).Value);
            var output = $"{timeSpan.Minutes:D2} : {timeSpan.Seconds:D2}";
            timerText.text = output;
        }
        else if (matchTimer.Expired(Runner))
        {
            MatchIsOver = true;
            matchTimer = TickTimer.None;
            OnGameIsOver?.Invoke();
            Debug.Log("match timer had ended");
        }
    }
    
}

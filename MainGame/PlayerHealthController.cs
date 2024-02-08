using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : NetworkBehaviour
{
    [SerializeField] private Animator bloodScreenHitAnimator;
    [SerializeField] private PlayerCameraController playerCameraController;

    [SerializeField] private Image fillAmountImage;
    //[SerializeField] private Image fillAmountImageLocal;

    //[SerializeField] private TextMeshProUGUI healthAmountText;
    [SerializeField] private ParticleSystem bloodEffect;
    [SerializeField] private Animator killUiAnimator;

    [Networked(OnChanged = nameof(HealthAmountChanged))]
    public int currentHealthAmount { get; set; }

    [Networked(OnChanged = nameof(OnBloodEffectStateChanged))]
    private NetworkBool playerBloodEffect { get; set; }
    

    [Networked] private int lastAttackerID { get; set; }
    public int attackerId = -1;

    [Networked] private Vector3 BulletPosition { get; set; }


    private const int MAX_HEALTH_AMOUNT = 100;

    private PlayerController playerController;

    public override void Spawned()
    {
        // called by only host
        playerController = GetComponent<PlayerController>();
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }

    //Since I also want to make late joined players to see other's health bar, i will use RPC
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_ReducePlayerHealth(int damage)
    {
        currentHealthAmount -= damage;
    }

    private static void HealthAmountChanged(Changed<PlayerHealthController> changed)
    {
        // update the visual and player data animation
        var currentHealth = changed.Behaviour.currentHealthAmount;
        // var currentlastattackerID = changed.Behaviour.lastAttackerID;
        changed.LoadOld(); // lead the previous data.

        var oldHealthAmout = changed.Behaviour.currentHealthAmount;

        // Only if the currentHealth is not same as the previous one
        if (currentHealth != oldHealthAmout)
        {
            changed.Behaviour.UpdateVisual(currentHealth);

            if (currentHealth != MAX_HEALTH_AMOUNT && currentHealth < oldHealthAmout)
            {
                changed.Behaviour.PlayerGoHit(currentHealth);
            }
        }

    }

    private void UpdateVisual(int healthamount)
    {
        var num = (float)healthamount / MAX_HEALTH_AMOUNT;
        fillAmountImage.fillAmount = num;
        //fillAmountImageLocal.fillAmount = num;
        //healthAmountText.text = $"{healthamount}/{MAX_HEALTH_AMOUNT}";
    }

    private void PlayerGoHit(int healthAmount)
    {
        if(!Object.HasStateAuthority){
            return;
        }

        if (healthAmount > 0)
        {
            //do blood hit animation, shake camera, 
            const string BLOOD_HIT_CLIP_NAME = "BloodScreenHit";
            bloodScreenHitAnimator.Play(BLOOD_HIT_CLIP_NAME);
            var shakeAmount = new Vector3(0.2f, 0.1f);
            playerCameraController.ShakeCamera(shakeAmount);
        }

        if (healthAmount <= 0 && !playerController.killed)
        {
            // kill the player
            var globalManagers = GlobalManagers.Instance;
            globalManagers.playerSpawnerController.CurrentSpawnedPlayers.TryGetValue(attackerId, out var attackerObj);

            playerController.KillPlayer(attackerObj);
            UpdatekillerandUpdateLeaderBoard();

            // var attackerHealthController = attackerObj.GetComponent<PlayerHealthController>();
            // attackerHealthController.Rpc_TriggerKillCount();
        }
    }

    private void UpdatekillerandUpdateLeaderBoard()
    {
        var globalManagers = GlobalManagers.Instance;
        //We try to grab our killer player object with our last attacker ID (which was set before, at DamagePLayer function)
        globalManagers.playerSpawnerController.CurrentSpawnedPlayers.TryGetValue(attackerId, out var attackerObj);
        if (Object.HasStateAuthority && attackerObj != null)
        {    
            Rpc_TargetedToPlayerKiller(attackerObj.StateAuthority, Utils.GetPlayerNickname(Object));
            //Making sure to modify it back to "NULL"
            lastAttackerID = -1;
            attackerId = -1;
        }

        //Update player scores (the player that died and the attacker)
        globalManagers.LeaderboardManager.UpdatePlayersScores(Object, attackerObj);


        //TriggerKillPopUpAnimation();

    }

    //Here is where the magic happens, we provide a damage number & and the attacker ID
    //The currentHealthAmount var is wrap under OnChanged (see below)
    private void DamagePlayer(int dmg, int attackerID)
    {
        this.lastAttackerID = attackerID;
        attackerId = attackerID;
        this.currentHealthAmount -= dmg;
        this.playerBloodEffect = true;
        StartCoroutine(StopBloodEffect());
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void Rpc_DamagePlayer(int damage, int attackerID)
    {
        if(attackerID != Object.StateAuthority.PlayerId){
            DamagePlayer(damage, attackerID);
        }
    }

    public void ResetHealthAmountToMax()
    {
        currentHealthAmount = MAX_HEALTH_AMOUNT;
    }

    public static void OnBloodEffectStateChanged(Changed<PlayerHealthController> changed)
    {
        var currentState = changed.Behaviour.playerBloodEffect;
        changed.LoadOld();
        var oldState = changed.Behaviour.playerBloodEffect;

        if (oldState != currentState)
        {
            // call function to change Blood Effect state.
            changed.Behaviour.playOrStopMuzzleEffect(currentState, changed.Behaviour.BulletPosition);
        }

    }

    private void playOrStopMuzzleEffect(bool play, Vector3 hitPosition)
    {
        if (play)
        {
            bloodEffect.Play();
            // hIT PRINT THE CURRENT LOCAL OF PLAY NOT TH HIT POINT
        }
        else
        {
            bloodEffect.Stop();
        }

        //StartCoroutine(StopBloodEffect());
        
    }

    private IEnumerator StopBloodEffect()
    {
        yield return new WaitForSeconds(0.1f);
        playerBloodEffect = false;
    }
    

    public void TriggerKillPopUpAnimation()
    {
        const string TRIGGER = "KillPopUp";
        killUiAnimator.SetTrigger(TRIGGER);
    }
    
    
    //Called on the TARGETED player, no need to check if this is local or not
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    private void Rpc_TargetedToPlayerKiller([RpcTarget] PlayerRef player, string playerThatDiedNickname)
    {
        //Show the kill UI for the local machine
        GlobalManagers.Instance.KillFeedManager.ShowKill(playerThatDiedNickname);
        Utils.PlayOneShot(Utils.SfxTypes.GotAKill);
    }
}

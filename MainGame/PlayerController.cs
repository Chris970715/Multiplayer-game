using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    /* 1. Once the player controller has been spawned, we get rigidbody component
    */
    public bool AcceptAnyInput => PlayerIsAlive && !GameManager.MatchIsOver;

    // Onchanged --> once it detects a change between the previous data and the current data.

    [Networked]
    public TickTimer RespawnTimer { get; private set; }

    [Networked(OnChanged = nameof(OnNicknameChanged))]
    public NetworkString<_32> playerName { get; set; }

    [Networked]
    private GunType.gunType PlayerControllerGunType { get; set; }

    [Networked]
    private NetworkButtons buttonPrev { get; set; }

    //To check if the player is still alive
    [Networked]
    public NetworkBool PlayerIsAlive { get; private set; }

    //To check if the player is still alive
    [Networked]
    public int PlayerId { get; private set; }

    [Networked]
    private Vector2 SeverNectSpawnPoint { get; set; }

    [Networked]
    private NetworkBool isGrounded { get; set; }

    public enum PlayerInputButtons
    {
        None,
        Jump,
        Shoot,
        Rope,
        OffRope
    }

    [SerializeField]
    private TextMeshProUGUI playerNameText;

    [SerializeField]
    private GameObject cam;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float jumpForce = 1000;

    [Header("Grounded Vars")]
    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private Transform groundDetectionObj;

    [Header("Item Vars")]
    [SerializeField]
    private LayerMask itemLayer;

    [SerializeField]
    private Transform itemDetectionObj;

    [Networked] NetworkString<_512> imageParamString {get; set;}

    private float horizontal;

    //Decided to use Rigidbody to move character
    private Rigidbody2D rigidbody;
    private PlayerWeaponController playerWeaponController;
    private PlayerVisualController playerVisualController;
    private PlayerHealthController playerHealthController;
    private PlayerKillDeathController playerKillDeathController;
    private PlayerSpriteController playerSpriteController;
    private GlobalManagers globalManagers;
    private PlayerNameSetter playerNameSetter;

    private float walkingSoundTimeStamp;

    private int remainingJumps;
    private const int maxJumps = 2; // Maximum number of jumps allowed

    // [Networked(OnChanged = nameof(OnKilledChanged))]
    // public bool killed { get; set; } = false;

    public bool killed;

    [Networked(OnChanged = nameof(OnDeadStatusChanged))]
    public bool isDead { get; set; }

    // the Spawned() method is from NetworkBehaviour class.
    // It gets called when the networked game object (the player) is spawned on the network.
    public override void Spawned()
    {
        globalManagers = GlobalManagers.Instance;
        globalManagers.playerSpawnerController.AddToEntry(
            Object.StateAuthority.PlayerId,
            this.Object
        );

        PlayerId = Object.StateAuthority.PlayerId;
        // globalManagers.playerSpawnerController.AddToEntry(Object.StateAuthority.PlayerId, this.Object);
        // assigning this for all of machines
        rigidbody = GetComponent<Rigidbody2D>();
        playerWeaponController = GetComponent<PlayerWeaponController>();
        playerVisualController = GetComponent<PlayerVisualController>();
        playerHealthController = GetComponent<PlayerHealthController>();
        playerKillDeathController = GetComponent<PlayerKillDeathController>();
        playerSpriteController = GetComponent<PlayerSpriteController>();

        //Registering for when player nickname has been set
        playerNameSetter = this.GetComponent<PlayerNameSetter>();
        playerNameSetter.OnNickNameSet += OnNicknameSet;

        SetLocalObjects();
        PlayerIsAlive = true;
    }

    private void OnNicknameSet(string nickName)
    {
        playerNameSetter.OnNickNameSet -= OnNicknameSet;
        globalManagers.LeaderboardManager.Rpc_AddPlayerToList(Object.StateAuthority.PlayerId);
    }

    private void SetLocalObjects()
    {
        if (Object.HasStateAuthority)
        {
            cam.transform.SetParent(null);
            cam.SetActive(true);

            RpcSetNickName(GlobalManagers.Instance.networkRunnerController.LocalPlayerNickName);

            if(GlobalManagers.Instance.WebGLManager.imageUrl != null && GlobalManagers.Instance.WebGLManager.imageUrl.Length > 0)
            {
                string[] parameters = GlobalManagers.Instance.WebGLManager.imageUrl.Split(',');
                string baseUrl = GlobalManagers.Instance.WebGLManager.baseUrl;
                // Debug.Log($"[SetLocalObjects]{baseUrl}/assets/collections/{parameters[0]}/playable/{parameters[1]}.png");
                string theUrl = $"{baseUrl}/assets/collections/{parameters[0]}/playable/{parameters[1]}.png";
                imageParamString = theUrl;
                playerSpriteController.Rpc_SendImageLInk(imageParamString);
            }
        }
    }

    //RPC is usually used to be used so the clients could actually tell the server something
    // this method is to tell server to change name
    [Rpc(sources: RpcSources.All, RpcTargets.StateAuthority)]
    private void RpcSetNickName(NetworkString<_32> nickName)
    {
        playerName = nickName;
    }

    [Rpc(sources: RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcGunState(GunType.gunType guntypeInput)
    {
        PlayerControllerGunType = guntypeInput;
    }

    // called every time for ticks to be checked (This method gets called every physics update for networked objects (akin to Unity's standard FixedUpdate).)
    public override void FixedUpdateNetwork()
    {
        CheckRespawnTimer();
        // will return false if the client does not have authority or Input authority
        // the requested type of input does not exist in the simulation
        //5 . get me the input from this player
        if (
            Runner.TryGetInputForPlayer<PlayerData>(Object.StateAuthority, out var input)
            && AcceptAnyInput
        )
        // if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input) && AcceptAnyInput)
        // if (GetInput(out PlayerData input))
        {
            rigidbody.velocity = new Vector2(
                input.HorizontalInput * moveSpeed,
                rigidbody.velocity.y
            );
            CheckJumpInput(input);
            //CheckRopeInput(input);
            buttonPrev = input.NetworkButtons;
        }

        playerVisualController.UpdateScalTransforms(rigidbody.velocity);
    }

    //the Following method get called after simulation(everything) frame rendering callback
    public override void Render()
    {
        playerVisualController.RendererVisuals(
            rigidbody.velocity,
            playerWeaponController.IsHoldingShootKey
        );
    }

    public void KillPlayer(NetworkObject attackerObj)
    {
        if (Object.HasStateAuthority)
        {   
            // Debug.Log($"[KillPlayer] Object.StateAuthority.PlayerId: {Object.StateAuthority.PlayerId} attackerObj.StateAuthority.PlayerId: {attackerObj.StateAuthority.PlayerId}");
            SeverNectSpawnPoint =
                GlobalManagers.Instance.playerSpawnerController.GetRandomSpawnPoint();
            // playerKillDeathController.Rpc_IncreasePlayerDeathAmount();

            playerWeaponController.itemBulletsCount.Type = GunType.gunType.HandGun;
            playerWeaponController.itemBulletsCount.GunSetting();
            playerWeaponController.playerItemController.playerGunSpriteChanged = GunType
                .gunType
                .HandGun;
            //playerWeaponController.itemBulletsCount.BulletCount = 12;
            PlayerIsAlive = false;
            killed = true;
            // Stop to simulate rigidbody of player
            rigidbody.simulated = false;
            //playerVisualController.TriggerDieAnimation();

            playerVisualController.TriggerDieAnimation();
            RespawnTimer = TickTimer.CreateFromSeconds(Runner, 6f);
            RPC_KillPlayer(true);
        }   
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RPC_KillPlayer(bool dead)
    {
        isDead = dead;
    }

    private static void OnDeadStatusChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.ChangeBodyConditionOnDeath();
    }

    // Turn on and off rigid body and rotation based on death condition
    public void ChangeBodyConditionOnDeath()
    {
        if(isDead){
            rigidbody.simulated = false;
            rigidbody.transform.Rotate(0, 0, -90);
        }else {
            rigidbody.simulated = true;
            rigidbody.transform.Rotate(0, 0, 90);
        }
    }

    // called before Fusion does any kind of networking applications
    // 2. Before Update (So every frame and before any network application the fusion does), we are checking  if we are local
    public void BeforeUpdate()
    {
        //Local machine
        // if (Runner.LocalPlayer == Object.HasInputAuthority && AcceptAnyInput)
        // if (AcceptAnyInput)
        // {
        // }
        const string HORIZONTAL = "Horizontal";
        //3. pass input from our keyboard
        horizontal = Input.GetAxisRaw(HORIZONTAL);
    }

    private void CheckJumpInput(PlayerData input)
    {
        isGrounded = (bool)
            Runner
                .GetPhysicsScene2D()
                .OverlapBox(
                    groundDetectionObj.transform.position,
                    groundDetectionObj.transform.localScale,
                    0,
                    groundLayer
                );

        if (isGrounded)
        {
            remainingJumps = maxJumps; // Reset the remaining jumps when grounded
        }

        var pressed = input.NetworkButtons.GetPressed(buttonPrev);
        if (pressed.WasPressed(buttonPrev, PlayerInputButtons.Jump) && remainingJumps > 0)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0); // Reset vertical velocity
            rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
            remainingJumps--;
        }
    }

    public PlayerData GetPlayerNetworkInput()
    {
        PlayerData data = new PlayerData();
        data.HorizontalInput = horizontal;
        // data.SpriteFlipped = playerWeaponController.FlipAuth;
        // data.GunPivotRotation = playerWeaponController.LocalQuaternionPivotRot;
        data.LocalMousePosition = Input.mousePosition;
        data.NetworkButtons.Set(PlayerInputButtons.Jump, Input.GetKey(KeyCode.Space));
        data.NetworkButtons.Set(PlayerInputButtons.Shoot, Input.GetButton("Fire1")); // left mouse click
        data.NetworkButtons.Set(PlayerInputButtons.Rope, Input.GetKeyDown(KeyCode.Q));
        data.NetworkButtons.Set(PlayerInputButtons.OffRope, Input.GetKeyUp(KeyCode.Q));
        return data;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        GlobalManagers.Instance.ObjectPoolingManager.RemoveNetworkObejctFromDic(Object);
        Destroy(gameObject);
    }

    private void SetPlayerNickname(NetworkString<_32> nickName)
    {
        playerNameText.text = nickName.ToString();
        globalManagers.LeaderboardManager.Rpc_AddPlayerToList(Runner.LocalPlayer.PlayerId);
    }

    // this will only be detected if Fusion detects any changes from previous onr to new one
    private static void OnNicknameChanged(Changed<PlayerController> changed)
    {
        var nickname = changed.Behaviour.playerName;
        changed.Behaviour.SetPlayerNickname(nickname);
    }

    private void CheckRespawnTimer()
    {
        if (PlayerIsAlive)
            return;

        if (RespawnTimer.Expired(Runner))
        {
            RespawnTimer = TickTimer.None;
            // perform respawn method
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        if (Object.HasStateAuthority)
        {
            PlayerIsAlive = true;
            killed = false;
            // Stop to simulate rigidbody of player
            rigidbody.simulated = true;
            rigidbody.position = SeverNectSpawnPoint;
            playerVisualController.TriggerRespawnAnimation();
            playerHealthController.ResetHealthAmountToMax();
            RPC_KillPlayer(false);
        }
    }

    private void Update()
    {
        var velocity = rigidbody.velocity;
        var isMoving = velocity.x > 0.1f || velocity.x < -0.1f;
        const float DELAY = 0.19f;

        if (isMoving && isGrounded && Time.time >= walkingSoundTimeStamp)
        {
            Utils.PlayOneShot(Utils.SfxTypes.Walk);
            walkingSoundTimeStamp = Time.time + DELAY;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
// using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
{
    const int NUMBEROFPELLETSINSHOTGUN = 10;
    
    public static PlayerWeaponController Instance { get; private set; }
    private PlayerController playerController;
    public PlayerItemController playerItemController;
    private PlayerVisualController playerVisualController;
    public ItemBulletsCount itemBulletsCount;

    public Quaternion LocalQuaternionPivotRot { get; private set;}
    
    public NetworkBool FlipAuth { get; private set; }

    [SerializeField] private NetworkPrefabRef bulletPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private NetworkPrefabRef snipperBulletPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private Transform firePointPos;
    
    [SerializeField] private float delayBetweenShoots = 0.10f;
    [SerializeField] private ParticleSystem muzzleEffect;
    [SerializeField] private Camera localCam;
    [SerializeField] private Transform pivotToRotate;

    [SerializeField] private SpriteRenderer gunSprite;
    [SerializeField] private Sprite[] items;

    [Header("Rifle cocking mechanism settings")]
    [SerializeField] private float fireRate_Rifle = 0.2f;
    [SerializeField] private float nextFireTime = 0f;
    
    [Header("Snipper cocking mechanism settings")] 
    [SerializeField] private float firerate_Snipper = 1f;
    [SerializeField] private float nextFireTime_Snipper = 0f;
    
    [Header("HandGun Settings")] 
    [SerializeField] private float fireRate_HandGun = 0.2f;
    [SerializeField] private float nextFireTime_HandGun = 0f;
    
    [Header("ShotGun Settings")]
    [SerializeField] private float fireRate_ShotGun = 2.5f;
    [SerializeField] private float nextFireTime_ShotGun = 0f;
    [SerializeField] private float shotGunSpreadAngle = 35f;

    [Header("EmptyGun Sound Time Settings")] 
    [SerializeField] private float emptyGunSoundTime;

    [Header("Bullets UI Settings")] 
    [SerializeField] private TextMeshProUGUI bulletUICount;
    
    //[Header("Reloading Amount")]
    //[SerializeField] private Image fillAmountImage;
    
    private Sprite currentGunSprite;

    [Networked, HideInInspector] public NetworkBool IsHoldingShootKey { get; private set; }
    [Networked (OnChanged = nameof(OnMuzzleEffectStateChanged))] private NetworkBool playerMuzzleEffect { get; set;}
    [Networked] private Quaternion currentPlayerPivotRotation { get; set; }
    [Networked] private NetworkButtons buttonPrev { get; set; }
    [Networked] private TickTimer shootCoolDown { get; set; }
    [Networked] private TickTimer EmptyCountDown { get; set; }
    [Networked] public NetworkBool ReloadingSoundCheck { get; set; }
    [Networked] private NetworkBool isSpriteFlipped { get; set; }
    
    [Networked] private NetworkBool PlayedEmptyGunSound { get; set; }
    

    
    private void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        playerController = GetComponent<PlayerController>();
        playerItemController = GetComponent<PlayerItemController>();
        playerVisualController = GetComponent<PlayerVisualController>();
        itemBulletsCount = GetComponent<ItemBulletsCount>();
    }

    public void BeforeUpdate()
    {
        // if (Runner.LocalPlayer == Object.HasInputAuthority && playerController.AcceptAnyInput)
        // {
        //     var direction = localCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //     var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //     //Now, it has to be sent to everyone in the room.
            
        //     LocalQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
        //     FlipAuth = rePositionGunSprite();

        // }
        PlayerRef _localPlayerRef = GlobalManagers.Instance.playerSpawnerController.localPlayerRef;
            
        if (Runner.TryGetInputForPlayer<PlayerData>(_localPlayerRef, out var input) && playerController.AcceptAnyInput)
        {
            var direction = localCam.ScreenToWorldPoint(input.LocalMousePosition) - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Now, it has to be sent to everyone in the room.
            
            LocalQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
            FlipAuth = rePositionGunSprite();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var _input) && playerController.AcceptAnyInput)

        if (Runner.TryGetInputForPlayer<PlayerData>(Object.StateAuthority, out var _input) && playerController.AcceptAnyInput)
        {   
            CheckShootInput(_input);
            //RpcUploadBulletCountUI();
            var direction = localCam.ScreenToWorldPoint(_input.LocalMousePosition) - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


            LocalQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
            FlipAuth = rePositionGunSprite();
            
            
            currentPlayerPivotRotation = LocalQuaternionPivotRot;
            
            buttonPrev = _input.NetworkButtons;

            isSpriteFlipped = FlipAuth;
        }

        // PlayerRef _localPlayerRef = GlobalManagers.Instance.playerSpawnerController.localPlayerRef;
        // if (Runner.LocalPlayer == _localPlayerRef && Runner.TryGetInputForPlayer<PlayerData>(_localPlayerRef, out var input) && playerController.AcceptAnyInput)
        // {   
            
        //     //RpcUploadBulletCountUI();
        //     var direction = localCam.ScreenToWorldPoint(input.LocalMousePosition) - transform.position;
        //     var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


        //     LocalQuaternionPivotRot = Quaternion.AngleAxis(angle, Vector3.forward);
        //     FlipAuth = rePositionGunSprite();
            
            
        //     currentPlayerPivotRotation = LocalQuaternionPivotRot;
            
        //     buttonPrev = input.NetworkButtons;

        //     isSpriteFlipped = FlipAuth;
        // }

        gunSprite.flipY = isSpriteFlipped;
        pivotToRotate.rotation = currentPlayerPivotRotation;
        
        if (Object.HasStateAuthority)
        // if (Runner.LocalPlayer == Object.HasInputAuthority)
        {
            RpcUploadBulletCountUI();
        }
    }

    private void CheckShootInput(PlayerData input) //  edit animation here 
    {
        var currentBtns = input.NetworkButtons.GetPressed(buttonPrev);

        IsHoldingShootKey = currentBtns.WasReleased(buttonPrev, PlayerController.PlayerInputButtons.Shoot);

        if (currentBtns.WasReleased(buttonPrev, PlayerController.PlayerInputButtons.Shoot) ) //&& shootCoolDown.ExpiredOrNotRunning(Runner)
        {
            //playerMuzzleEffect = true;
            //shootCoolDown = TickTimer.CreateFromSeconds(Runner, delayBetweenShoots);

            //Runner.Spawn(bulletPrefab, firePointPos.position, firePointPos.rotation, Object.InputAuthority);
            PickWaysToSpawnBullets();

        }
        else
        {
            //playerMuzzleEffect = false;
        }
    }

    private static void OnMuzzleEffectStateChanged(Changed<PlayerWeaponController> changed)
    {
        var currentState = changed.Behaviour.playerMuzzleEffect;
        changed.LoadOld();
        
        var oldState = changed.Behaviour.playerMuzzleEffect;

        if (oldState != currentState)
        {
            changed.Behaviour.playOrStopMuzzleEffect(currentState);
        }
    }
    
    private Vector3 HandleAngle(float angle)
    {
        Vector3 localScale = Vector3.one;

        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = +1f;
        }
        
        return localScale;
    }
    
    private NetworkBool rePositionGunSprite()
    {
        Vector3 mouseposition;
        Vector3 aimDirection;
        
        if(Runner.TryGetInputForPlayer<PlayerData>(GlobalManagers.Instance.playerSpawnerController.localPlayerRef, out var input)){
            mouseposition = localCam.ScreenToWorldPoint(input.LocalMousePosition);
        }else {
            mouseposition = localCam.ScreenToWorldPoint(Input.mousePosition);
        }
            aimDirection = (mouseposition - firePointPos.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            
        // If the angle is within certain bounds, flip the sprite horizontally
        if (angle > 90 || angle < -90)
        {
            return true; // Set the networked property
        }
        else
        {
            return false; // Set the networked property
        }

        //gunSprite.transform.localScale = GunPositionHandler;
    }

    private void playOrStopMuzzleEffect(bool play)
    {
        if (play)
        {
            muzzleEffect.Play();
        }
        else
        {
            muzzleEffect.Stop();
        }
    }

    private void PickWaysToSpawnBullets()
    {
        switch (playerItemController.playerGunSpriteChanged) //playerItemController.playerGunSpriteChanged
        {
            case GunType.gunType.Rifle:
                if (itemBulletsCount.BulletCount > 0)
                {
                    if (shootCoolDown.ExpiredOrNotRunning(Runner))
                    {
                        shootCoolDown = TickTimer.CreateFromSeconds(Runner, fireRate_Rifle);
                        Runner.Spawn(bulletPrefab, firePointPos.position, pivotToRotate.rotation,
                            Object.InputAuthority);
                        playerMuzzleEffect = true;
                        playerVisualController.ShootingRemdererVisual(true);
                        StartCoroutine(StopShootingAnimation());
                        itemBulletsCount.BulletCount--;
                    }
                    
                }
                else
                {
                    if (EmptyCountDown.ExpiredOrNotRunning(Runner))
                    {
                        EmptyCountDown = TickTimer.CreateFromSeconds(Runner, emptyGunSoundTime);
                        Utils.PlayOneShot(Utils.SfxTypes.EmptyGun);
                    }
                }
                

                break;
            case GunType.gunType.HandGun:
                if (itemBulletsCount.BulletCount > 0)
                {
                    if (shootCoolDown.ExpiredOrNotRunning(Runner))
                    {
                        shootCoolDown = TickTimer.CreateFromSeconds(Runner, fireRate_HandGun);
                        Runner.Spawn(bulletPrefab, firePointPos.position, firePointPos.rotation, Object.InputAuthority);
                        playerMuzzleEffect = true;
                        playerVisualController.ShootingRemdererVisual(true);
                        itemBulletsCount.BulletCount--;
                        StartCoroutine(StopShootingAnimation());
                    }
                }
                else
                {
                    if (EmptyCountDown.ExpiredOrNotRunning(Runner))
                    {
                        EmptyCountDown = TickTimer.CreateFromSeconds(Runner, emptyGunSoundTime);
                        Utils.PlayOneShot(Utils.SfxTypes.EmptyGun);
                    }
                }
                
                break;
            
            case GunType.gunType.ShotGun:
                if (itemBulletsCount.BulletCount > 0)
                {
                    if (shootCoolDown.ExpiredOrNotRunning(Runner))
                    {
                        shootCoolDown = TickTimer.CreateFromSeconds(Runner, fireRate_ShotGun);
                        playerMuzzleEffect = true;

                        for (int i = 0; i < NUMBEROFPELLETSINSHOTGUN; i++)
                        {
                            float spread = Random.Range(-shotGunSpreadAngle / 2, shotGunSpreadAngle / 2);

                            // Use the firepoint's upward direction as the base direction.
                            Vector3 baseDirection = firePointPos.up;

                            // Calculate the spread direction by applying the spread rotation to the base direction.
                            Vector3 spreadDirection = Quaternion.Euler(0, 0, spread) * baseDirection;

                            // Determine the rotation for the bullet so it faces the direction it will travel.
                            Quaternion bulletRotation = Quaternion.LookRotation(Vector3.forward, spreadDirection);


                            Runner.Spawn(bulletPrefab, firePointPos.position, bulletRotation, Object.InputAuthority);
                            
                        }
                        
                        playerVisualController.ShootingRemdererVisual(true);
                        Utils.PlayOneShot(Utils.SfxTypes.Shoot);
                        itemBulletsCount.BulletCount--;
                        StartCoroutine(StopShootingAnimation());

                    }
                    if (ReloadingSoundCheck)
                    {
                        Utils.PlayOneShot(Utils.SfxTypes.ShotGunReloading);
                        ReloadingSoundCheck = false;
                    }
                }
                else
                {
                    if (EmptyCountDown.ExpiredOrNotRunning(Runner))
                    {
                        EmptyCountDown = TickTimer.CreateFromSeconds(Runner, emptyGunSoundTime);
                        Utils.PlayOneShot(Utils.SfxTypes.EmptyGun);
                    }
                }
                break;
            
            case GunType.gunType.SnipperGun:
                if (itemBulletsCount.BulletCount > 0)
                {
                    if (shootCoolDown.ExpiredOrNotRunning(Runner))
                    {
                        shootCoolDown = TickTimer.CreateFromSeconds(Runner, firerate_Snipper);
                        Runner.Spawn(snipperBulletPrefab, firePointPos.position, firePointPos.rotation,
                            Object.InputAuthority);
                        playerMuzzleEffect = true;
                        playerVisualController.ShootingRemdererVisual(true);
                        itemBulletsCount.BulletCount--;
                        StartCoroutine(StopShootingAnimation());
                    }
                    else
                    {
                        if (ReloadingSoundCheck)
                        {
                            Utils.PlayOneShot(Utils.SfxTypes.SnipperGunReloading);
                            ReloadingSoundCheck = false;
                        }

                    }
                }
                else
                {
                    if (EmptyCountDown.ExpiredOrNotRunning(Runner))
                    {
                        EmptyCountDown = TickTimer.CreateFromSeconds(Runner, emptyGunSoundTime);
                        Utils.PlayOneShot(Utils.SfxTypes.EmptyGun);
                    }
                }

                break;
        }
        
    }


    
    private IEnumerator StopShootingAnimation()
    {
        ReloadingSoundCheck = true;
        yield return new WaitForSeconds(0.1f);  // Adjust the duration based on your animation's length
        playerVisualController.ShootingRemdererVisual(false);
        playerMuzzleEffect = false;
    }

    [Rpc(sources: RpcSources.All, RpcTargets.StateAuthority)]
    private void RpcUploadBulletCountUI()
    {
        bulletUICount.enabled = true;
        bulletUICount.text = itemBulletsCount.BulletCount.ToString();
    }

    


    /*
    private void ShootSound()
    {
        const float DELAY = 0.125f;

        if (Time.time >= walkingSoundTimeStamp)
        {
            Utils.PlayOneShot(Utils.SfxTypes.Shoot);
            walkingSoundTimeStamp = Time.time + DELAY;
        }
    }
    */
     


    
}

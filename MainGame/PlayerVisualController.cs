using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisualController : MonoBehaviour
{
    public static PlayerVisualController Instance { get; private set; }
    
    // responsible for visual part of player

    [SerializeField] private Animator WholeAnimator; // Animator in Graphics
    [SerializeField] private Animator walkAnimator;
    [SerializeField] private Animator shootAnimator;

    [SerializeField] private Transform pivotGun;
    [SerializeField] private Transform canvasTr;
    
    // ReloadingBar
    [SerializeField] private Slider slider;
    [SerializeField] private float fireRateFromPlayerAim = 0f;
    
    
    // Flipping Player
    private bool isFacingRight = true;
    private bool init;
    private Vector3 originalPlayerScale;
    private Vector3 originalGunPivotScale;
    private Vector3 originalCavasScale;

    private readonly int isMovingHash = Animator.StringToHash("IsWalking");
    private readonly int isShootingHash = Animator.StringToHash("IsShooting");

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        originalCavasScale = canvasTr.transform.localScale;
        originalGunPivotScale = pivotGun.transform.localScale;
        originalPlayerScale = this.transform.localScale;
        
        const int SHOOTING_LAYER_INDEX = 1;
        //animator.SetLayerWeight(SHOOTING_LAYER_INDEX,1);
        
        init = true;
    }

    //called from PlayerController class
    public void RendererVisuals(Vector2 velocity, bool isShooting) //, bool isShooting
    {
        if (!init) return;
        
        var isMoving = velocity.x > 0.1f || velocity.x < -0.1f;
        
        walkAnimator.SetBool(isMovingHash, isMoving);

        //shootAnimator.SetBool(isShootingHash, isShooting);
    }

    public void ShootingRemdererVisual(bool isShooting)
    {
        shootAnimator.SetBool(isShootingHash, isShooting);
    }
    
    
    public void TriggerDieAnimation()
    {
        const string TRIGGER = "Die";
        WholeAnimator.SetTrigger(TRIGGER);
    }
    
    public void TriggerRespawnAnimation()
    {
        const string TRIGGER = "Respawn";
        WholeAnimator.SetTrigger(TRIGGER);
    }

    public void UpdateScalTransforms(Vector2 velocity)
    {
        if (!init) return;
        
        if (velocity.x > 0.1f)
        {
            isFacingRight = true;
        }
        else if (velocity.x < -0.1f)
        {
            isFacingRight = false;
        }

        SetObjectLocalScaleBasedOnDir(gameObject, originalPlayerScale);
        SetObjectLocalScaleBasedOnDir(canvasTr.gameObject, originalCavasScale);
        SetObjectLocalScaleBasedOnDir(pivotGun.gameObject, originalGunPivotScale);
    }
    

    private void SetObjectLocalScaleBasedOnDir(GameObject obj, Vector3 originalScale)
    {
        var yValue = originalScale.y;
        var zValue = originalScale.z;
        var xValue = isFacingRight ? originalScale.x : -originalScale.x;
        
        obj.transform.localScale = new Vector3(xValue, yValue, zValue);
    }

    public void EditReloadingValue(float fireRate)
    {
        fireRateFromPlayerAim = fireRate;
    }
    
    public void SetUpReloadingBar(float reloadingTotalValue, float reloadingValue)
    {
        slider.maxValue = reloadingTotalValue;
        slider.value = reloadingValue;
    }
}

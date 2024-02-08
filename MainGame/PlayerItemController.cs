using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemController : NetworkBehaviour
{

    [SerializeField] private SpriteRenderer playerGunSprite;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private Sprite[] gunUISprites;
    [SerializeField] private Image gunUISprite;

    [Networked(OnChanged = nameof(OnplayerGunSpriteChanged))] public GunType.gunType playerGunSpriteChanged { get; set;}

    private GunType playerGunType;
    

    public override void Spawned()
    {
        //Setting playerGunType to basic, Rifle
        gunUISprite.enabled = false;
        playerGunSpriteChanged = GunType.gunType.HandGun;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_ActivateGunSprite(GunType.gunType input)
    {
        playerGunSpriteChanged = input;
    }
    
    private static void OnplayerGunSpriteChanged(Changed<PlayerItemController> changed)
    {
        changed.Behaviour.UpdateVisual();
    }

    private void UpdateVisual()
    {
        switch (playerGunSpriteChanged)
        {
            case GunType.gunType.Rifle:
                if (Object.HasInputAuthority)
                {
                    gunUISprite.enabled = true;
                    gunUISprite.sprite = gunUISprites[0];
                    
                }
                playerGunSprite.sprite = itemSprites[0];
                break;
            case GunType.gunType.HandGun:
                if (Object.HasInputAuthority)
                {
                    gunUISprite.enabled = true;
                    gunUISprite.sprite = gunUISprites[1];
                    
                }
                playerGunSprite.sprite = itemSprites[1];
                break;
            case GunType.gunType.ShotGun:
                if (Object.HasInputAuthority)
                {
                    gunUISprite.enabled = true;
                    gunUISprite.sprite = gunUISprites[2];
                   
                }
                playerGunSprite.sprite = itemSprites[2];
                break;
            case GunType.gunType.SnipperGun:
                if (Object.HasInputAuthority)
                {
                    gunUISprite.enabled = true;
                    gunUISprite.sprite = gunUISprites[3];
                 
                }
                playerGunSprite.sprite = itemSprites[3];
                break;
        }
    }
    
    
}

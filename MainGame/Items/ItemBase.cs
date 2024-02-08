using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : NetworkBehaviour
{
    [Header("Item Base Settings")]
    [SerializeField]
    protected LayerMask plaLayerMask;

    [SerializeField]
    protected float liveTimeAmount;

    protected Collider2D coll;
    protected bool itemTouched = false;

    [Networked]
    protected NetworkBool didithitsomething { get; set; }

    public override void Spawned()
    {
        coll = GetComponent<Collider2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!didithitsomething)
        {
            CheckIfWeHitAPlayer();
        }

        if (didithitsomething && !itemTouched)
        {
            Utils.PlayOneShot(Utils.SfxTypes.GotAItem);
            Runner.Despawn(Object);
            itemTouched = true;
        }
    }

    protected virtual void CheckIfWeHitAPlayer()
    {
        Collider2D playerCollider = Runner
            .GetPhysicsScene2D()
            .OverlapBox(transform.position, coll.bounds.size, 0, plaLayerMask);
        if (playerCollider)
        {
            var player = playerCollider.GetComponentInParent<NetworkObject>();
            // var didNotHitOurOwnPlayer = player.StateAuthority.PlayerId != Runner.LocalPlayer;

            if (player)
            {
                player
                    .GetComponent<PlayerItemController>()
                    .Rpc_ActivateGunSprite(GunType.gunType.Rifle);

                didithitsomething = true;
            }
        }

        // if (hits.Count > 0)
        // {
        //     foreach (var item in hits)
        //     {
        //         if (item.Hitbox != null)
        //         {
        //             var player = item.Hitbox.GetComponentInParent<NetworkObject>();
        //             var didNotHitOurOwnPlayer = player.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;

        //             if (didNotHitOurOwnPlayer)
        //             {

        //                 if (Runner.IsServer)
        //                 {
        //                     player.GetComponent<PlayerItemController>().Rpc_ActivateGunSprite(GunType.gunType.Rifle);
        //                 }

        //                 didithitsomething = true;
        //                 break;
        //             }
        //         }
        //     }
        // }
    }
}

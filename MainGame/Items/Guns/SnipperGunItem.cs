using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SnipperGunItem : ItemBase 
{
    protected override void CheckIfWeHitAPlayer()
    {
        Collider2D playerCollider = Runner
            .GetPhysicsScene2D()
            .OverlapBox(transform.position, coll.bounds.size, 0, plaLayerMask);

        if (playerCollider)
        {
            var player = playerCollider.GetComponentInParent<NetworkObject>();
            var didNotHitOurOwnPlayer =
                player.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;

            if (player)
            {
                player
                    .GetComponent<PlayerItemController>()
                    .Rpc_ActivateGunSprite(GunType.gunType.SnipperGun);
                player.GetComponent<ItemBulletsCount>().Type = GunType.gunType.SnipperGun;
                player.GetComponent<ItemBulletsCount>().GunSetting();
                player.GetComponent<PlayerController>().RpcGunState(GunType.gunType.SnipperGun);

                didithitsomething = true;
                itemTouched = false;
            }
        }
    }
}

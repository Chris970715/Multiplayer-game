using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class AssultRifleItem : ItemBase
{
    protected override void CheckIfWeHitAPlayer()
    {
        Collider2D playerCollider = Runner
            .GetPhysicsScene2D()
            .OverlapBox(transform.position, coll.bounds.size, 0, plaLayerMask);

        if (playerCollider)
        {
            var player = playerCollider.GetComponentInParent<NetworkObject>();
            if (player)
            {
                player
                    .GetComponent<PlayerItemController>()
                    .Rpc_ActivateGunSprite(GunType.gunType.Rifle);
                player.GetComponent<ItemBulletsCount>().Type = GunType.gunType.Rifle;
                player.GetComponent<ItemBulletsCount>().GunSetting();
                player.GetComponent<PlayerController>().RpcGunState(GunType.gunType.Rifle);
                didithitsomething = true;
                itemTouched = false;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemBullet : ItemBase
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
                switch (player.GetComponent<ItemBulletsCount>().Type)
                {
                    case GunType.gunType.Rifle:
                        player.GetComponent<ItemBulletsCount>().BulletCount = 30;
                        break;
                    case GunType.gunType.HandGun:
                        player.GetComponent<ItemBulletsCount>().BulletCount = 12;
                        break;
                    case GunType.gunType.ShotGun:
                        player.GetComponent<ItemBulletsCount>().BulletCount = 8;
                        break;
                    case GunType.gunType.SnipperGun:
                        player.GetComponent<ItemBulletsCount>().BulletCount = 6;
                        break;
                }
                itemTouched = false;
                didithitsomething = true;
            }
        }
    }

}

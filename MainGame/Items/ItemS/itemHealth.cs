using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

 public class itemHealth : ItemBase
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
                var currentBloodAmount = player.GetComponent<PlayerHealthController>().currentHealthAmount + 50;

                player.GetComponent<PlayerHealthController>().currentHealthAmount = currentBloodAmount > 100 ? 100 : currentBloodAmount;

                itemTouched = false;
                didithitsomething = true;
            }
        }
    }
} 
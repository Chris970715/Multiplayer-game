using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [Header("Bullet Base Settings")] 
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected LayerMask plaLayerMask;
    [SerializeField] protected int bulletDamage;
    
    [SerializeField] protected float moveSpeed = 20f;
    [SerializeField] protected float liveTimeAmount = 0.8f;
    [Networked] protected NetworkBool didithitsomething { get; set; }
    [Networked] protected TickTimer lifeTimeTimer { get; set; }

    protected Collider2D coll;
    protected float walkingSoundTimeStamp;

    public override void Spawned()
    {
        coll = GetComponent<Collider2D>();
        lifeTimeTimer = TickTimer.CreateFromSeconds(Runner, liveTimeAmount);
        SettingBulletDamage();
        Utils.PlayOneShot(Utils.SfxTypes.Shoot);
        //ShootSound();
    }

    public override void FixedUpdateNetwork()
    {
        if (!didithitsomething)
        {
            CheckIfHitGround();
            CheckIfWeHitAPlayer();
        }

        if (lifeTimeTimer.ExpiredOrNotRunning(Runner) == false && !didithitsomething)
        {
            transform.Translate(transform.right * moveSpeed * Runner.DeltaTime, Space.World);
        }

        if (lifeTimeTimer.Expired(Runner) || didithitsomething)
        {
            lifeTimeTimer = TickTimer.None;
            Runner.Despawn(Object);
        }
    }

    public void CheckIfHitGround()
    {

        var groundCollider = Runner.GetPhysicsScene2D().OverlapBox(transform.position,
            coll.bounds.size,
            0,
            groundMask
        );

        if (groundCollider != default)
        {
            didithitsomething = true;
        }
    }

    protected List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    
    protected void CheckIfWeHitAPlayer()
    {
        // Runner.LagCompensation.OverlapBox(transform.position, coll.bounds.size, Quaternion.identity,
        //     Object.InputAuthority, hits, plaLayerMask);
        Collider2D playerCollider = Runner
            .GetPhysicsScene2D()
            .OverlapBox(transform.position, coll.bounds.size, 0, plaLayerMask);

        if (playerCollider)
        {
            var player = playerCollider.GetComponentInParent<PlayerController>();

            //Object.StateAuthority.PlayerId is the shooter
            if (player && player.PlayerIsAlive && player.PlayerId != Object.StateAuthority.PlayerId)
            {
                var playerHealthController = player.GetComponent<PlayerHealthController>();
                playerHealthController.Rpc_DamagePlayer(bulletDamage, Object.StateAuthority.PlayerId);
                didithitsomething = true;
            }
        }
    }

    protected virtual void SettingBulletDamage()
    {
        bulletDamage = 5;
    }
    
    protected void ShootSound()
    {
        const float DELAY = 0.1f;

        if (Time.time >= walkingSoundTimeStamp)
        {
            Utils.PlayOneShot(Utils.SfxTypes.Shoot);
            walkingSoundTimeStamp = Time.time + DELAY;
        }
    }
    

}

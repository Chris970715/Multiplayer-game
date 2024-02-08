using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SnipperBullet : Bullet
{
    public override void Spawned()
    {
        coll = GetComponent<Collider2D>();
        lifeTimeTimer = TickTimer.CreateFromSeconds(Runner, liveTimeAmount);
        SettingBulletDamage();
        Utils.PlayOneShot(Utils.SfxTypes.SnipperShot);
        //ShootSound();
    }
    
    protected override void SettingBulletDamage()
    {
        bulletDamage = 100;
    }
}

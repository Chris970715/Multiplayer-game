using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemBulletsCount : NetworkBehaviour
{
    [Networked, HideInInspector] public GunType.gunType Type { get; set; }
    [Networked, HideInInspector] public int BulletCount { get; set; }

    
    public override void Spawned()
    {
        Type = GunType.gunType.HandGun;
        GunSetting();
    }

    public void GunSetting()
    {
        switch (Type)
        {
            case GunType.gunType.Rifle:
                BulletCount = 30;
                break;
            
            case GunType.gunType.HandGun:
                BulletCount = 12;
                break;
            
            case GunType.gunType.ShotGun:
                BulletCount = 8;
                break;
            
            case GunType.gunType.SnipperGun:
                BulletCount = 6;
                break;
        }
    }
    
    
}

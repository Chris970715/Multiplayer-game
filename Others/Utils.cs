using System.Collections;
using UnityEngine;
using Fusion;

public static class Utils
{
    // This class is designed to help other classes 
    // It is gonna save us from duplicating codes.
    
    // Method that waits the length of the animation 
    // Method that disables or enables, depending on the state what we are passing

    public static IEnumerator PlayAnimationAndSetStateWhenFinished(GameObject parent, Animator animator,
        string clipName, bool activeStateAtTheEnd = true)
    {
        animator.Play(clipName); // Play the animation

        var animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        
        yield return new WaitForSecondsRealtime(animationLength);
        
        parent.SetActive(activeStateAtTheEnd);

    }
    
    public static bool IsLocalPlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        return runner.LocalPlayer == playerRef;
    }
    
    public enum SfxTypes
    {
        None,
        Jump,
        Shoot,
        SnipperShot,
        Walk,
        EmptyGun,
        RifleReloading,
        ShotGunReloading,
        HandGunReloading,
        SnipperGunReloading,
        GotAItem,
        GotHit,
        Die,
        GotAKill,
        HitSomeone,
        Click,
        BulletHitAnything,
        Pop
    }
    
    public static void PlayOneShot(SfxTypes type)
    {
        GlobalManagers.Instance.AudioController.PlayOneShot(type);
    }
    
    //Grabs the nickname from the PlayerNameSetter comp
    public static string GetPlayerNickname(NetworkObject obj)
    {
        var nickname = string.Empty;

        obj.TryGetBehaviour<PlayerNameSetter>(out var name);
        if (name != null)
        {
            nickname = name.NickName.Value;
        }

        return nickname;
    }
}

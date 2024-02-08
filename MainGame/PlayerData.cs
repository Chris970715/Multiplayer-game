using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct PlayerData : INetworkInput
{
    // this struct will contain any player input data that we need to sync.

    public float HorizontalInput;
    public Vector3 LocalMousePosition;
    public NetworkButtons NetworkButtons;
    
}
 
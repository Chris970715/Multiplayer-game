using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner2D;
    

    private void Start()
    {
        cinemachineConfiner2D.m_BoundingShape2D = GlobalManagers.Instance.GameManager.cameraBounds;
    }

    public void ShakeCamera(Vector3 shakeAmount)
    {
        impulseSource.GenerateImpulse(shakeAmount);
    }
    
}
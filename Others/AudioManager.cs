using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set;}
    
    [Header("Button Audio Settings")] 
    [SerializeField] private AudioClip btnAudioClip;
    [SerializeField] [Range(0f,1f)] private float btnAudioClipVolume = 1f;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayButtonClip()
    {
        if (!btnAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(btnAudioClip, Camera.main.transform.position, btnAudioClipVolume);
        }
    }
    
    
}

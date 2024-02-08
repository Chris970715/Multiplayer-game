using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private SfxData[] allSoundEffects;
    private AudioSource audioSource;

    private void Start()
    {
        GlobalManagers.Instance.AudioController = this;
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayOneShot(Utils.SfxTypes type)
    {
        if (allSoundEffects?.Length > 0)
        {
            foreach (var sfxData in allSoundEffects)
            {
                if (sfxData.SfxType == type)
                {
                    audioSource.PlayOneShot(sfxData.AudioClip, sfxData.Volume);
                    break;
                }
            }
        }
    }

    [Serializable]
    public class SfxData
    {
        [field: SerializeField] public Utils.SfxTypes SfxType { get; private set; }
        [field: SerializeField] public AudioClip AudioClip { get; private set; }
        [field: SerializeField] public float Volume = 1f;
    }
}

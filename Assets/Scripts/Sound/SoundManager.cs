using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    MELEE,
    SHOOT,
    DAMAGED,
    DASH,
    DEATH,
    RELOADING
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType soundType, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)soundType], volume);
    }
}

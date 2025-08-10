using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioClip deadSFX;
    private AudioSource audioSource;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if(audioSource == null) 
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayAttack()
    {
        PlaySFX(attackSFX);
    }

    public void DeadSFX()
    {
        PlaySFX(deadSFX);
    }

    private void PlaySFX(AudioClip clip)
    {
        if(clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

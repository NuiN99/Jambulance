using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource generalSource;
    [SerializeField] AudioSource musicSource;
    
    public static AudioController Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        generalSource.PlayOneShot(clip, volume);
    }

    public void PlaySpatialSound(AudioClip clip, Vector3 pos, float volume)
    {
        AudioSource.PlayClipAtPoint(clip, pos, volume);
    }

    public void PlayMusic(AudioClip clip, float volume)
    {
        musicSource.PlayOneShot(clip, volume);
    }
}

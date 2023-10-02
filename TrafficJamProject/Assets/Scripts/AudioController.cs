using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource generalSource;
    [SerializeField] AudioSource musicSource;

    [SerializeField] AudioClip gameOverSound;

    [SerializeField] AudioClip music;

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

    private void OnEnable()
    {
        GameController.OnPlayerDeath += PlayGameOverSound;
        GameController.OnGameStarted += OnGameStart;
        GameController.OnPlayerWon += OnGameWin;
    }
    private void OnDisable()
    {
        GameController.OnPlayerDeath -= PlayGameOverSound;
        GameController.OnGameStarted -= OnGameStart;
        GameController.OnPlayerWon += OnGameWin;
    }

    private void Start()
    {
        Tween.AudioVolume(generalSource, 0, generalSource.volume, 1f, Ease.InQuart);
        Tween.AudioVolume(musicSource, 0, musicSource.volume, 1f, Ease.InQuart);
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

    void PlayGameOverSound()
    {
        float startVol = musicSource.volume;
        Tween.AudioVolume(musicSource, 0, 5, Ease.InOutSine)
        .OnComplete(() =>
        {
            Tween.AudioVolume(generalSource, 0, 2f, Ease.InOutSine);
            musicSource.clip = gameOverSound;
            musicSource.Play();
            Tween.AudioVolume(musicSource, startVol, 2, Ease.InOutSine);
        });
    }

    void OnGameStart()
    {
        musicSource.Play();
    }

    void OnGameWin()
    {
        Tween.AudioVolume(musicSource, 0, 3, Ease.InOutSine);
    }
}

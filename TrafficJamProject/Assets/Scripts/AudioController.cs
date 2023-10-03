using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource generalSource;
    [SerializeField] AudioSource musicSource;
    [SerializeField] GameObject spatialSource;
    [SerializeField] AudioClip gameOverSound;

    [SerializeField] AudioClip musicClip;

    [SerializeField] AudioClip music;

    public float masterVolume = 1;
    [SerializeField] AudioMixer mixer;

    [SerializeField] GameObject volSlider;

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
        GameController.OnPlayerWon -= OnGameWin;
    }

    private void Start()
    {
        Tween.AudioVolume(generalSource, 0, generalSource.volume, 1f, Ease.InQuart);
        Tween.AudioVolume(musicSource, 0, musicSource.volume, 1f, Ease.InQuart);

        if (ES3.KeyExists("Volume")) 
        {
            masterVolume = ES3.Load<float>("Volume");
            volSlider.GetComponent<UnityEngine.UI.Slider>().value = masterVolume;
        }
        mixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
    }

    public void SetVolume(float vol)
    {
        masterVolume = vol;
        ES3.Save("Volume", masterVolume);
        mixer.SetFloat("MasterVolume", Mathf.Log10(vol) * 20);
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        generalSource.PlayOneShot(clip, volume * 3);
    }

    public void PlaySpatialSound(AudioClip clip, Vector3 pos, float volume)
    {
        GameObject spatial = Instantiate(spatialSource, pos, Quaternion.identity, transform);
        AudioSource source = spatial.GetComponent<AudioSource>();
        source.PlayOneShot(clip, volume * 3);

        Destroy(spatial, 3f);
    }

    void PlayGameOverSound()
    {
        float startVol = musicSource.volume;
        Tween.AudioVolume(musicSource, 0, 1.5f, Ease.InOutSine)
        .OnComplete(() =>
        {
            musicSource.clip = gameOverSound;
            musicSource.Play();
            Tween.AudioVolume(musicSource, startVol, 1, Ease.InOutSine);
        });
    }

    void OnGameStart()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    void OnGameWin()
    {
        float startVol = musicSource.volume;
        Tween.AudioVolume(musicSource, 0, 1.5f, Ease.InOutSine)
        .OnComplete(() =>
        {
            musicSource.clip = gameOverSound;
            musicSource.Play();
            Tween.AudioVolume(musicSource, startVol, 1, Ease.InOutSine);
        });

        /*Tween.Custom(masterVolume, 0.0001f, 2f, (val) =>
        {
            mixer.SetFloat("MasterVolume", Mathf.Log10(val) * 20);
        });*/
    }

    public void UpdateVolume(float value)
    {
        generalSource.volume = value;
        musicSource.volume = value;
    }

}

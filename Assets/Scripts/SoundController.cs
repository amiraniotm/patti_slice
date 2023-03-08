using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource, sfxSource;
    [SerializeField] private AudioClip[] availableMusic;
    [SerializeField] private MasterController masterController;

    private bool musicPaused;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.clip = availableMusic[0];
        PlayMusic();
    }

    public void SetCurrentMusicClip()
    {
        int currentMusicKey = masterController.currentLevelKey;
        musicSource.clip = availableMusic[currentMusicKey];
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void TogglePauseMusic()
    {
        if(!musicPaused) {
            musicSource.Pause();
        } else {
            musicSource.UnPause();
        }

        musicPaused = !musicPaused;
    }

    public void PlaySound(AudioClip sound, float volume = 1.0f)
    {
        sfxSource.PlayOneShot(sound, volume);
    }
}

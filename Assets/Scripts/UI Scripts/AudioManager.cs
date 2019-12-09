using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    //VARIABLES
    #region Static Instance
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    instance = new GameObject("Spawned AudioManager", typeof(AudioManager)).GetComponent<AudioManager>();
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion
    #region Fields
    private AudioSource musicSource;
    private AudioSource musicSource2;
    private AudioSource sfxSource;
    #endregion
    private bool musicSourcePlaying;

    //UPDATES
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        musicSource = this.gameObject.AddComponent<AudioSource>();
        musicSource2 = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource2.loop = true;
    }

    //METHODS
    public void PlayMusic(AudioClip musicClip, float volume)
    {
        //Determine which music source is active
        AudioSource activeSource = (musicSourcePlaying) ? musicSource : musicSource2;

        activeSource.clip = musicClip;
        activeSource.volume = volume;
        activeSource.Play();
    }
    public void PlayMusicWithFade(AudioClip newClip, float transitionTime = 1f)
    {
        AudioSource activeSource = (musicSourcePlaying) ? musicSource : musicSource2;
        StartCoroutine(UpdateMusicWithFade(activeSource, newClip, transitionTime));
    }
    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime = 1f)
    {
        if(!activeSource.isPlaying)
        {
            activeSource.Play();
            float t = 0.0f;

            //Fade out
            for(t = 0; t < transitionTime; t += Time.deltaTime)
            {
                activeSource.volume = (1 - (t / transitionTime));
                yield return null;
            }

            activeSource.Stop();
            activeSource.clip = newClip;
            activeSource.Play();

            //Fade in
            for (t = 0; t < transitionTime; t += Time.deltaTime)
            {
                activeSource.volume = ((t / transitionTime));
                yield return null;
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        //OneShot allows for overlapping sounds, unlike Play which cuts the clip short
        sfxSource.PlayOneShot(clip);
    }
    public void PlaySFX(AudioClip clip, float volume)
    {
        //Adjusted for Sound volume
        sfxSource.PlayOneShot(clip, volume);
    }
    
    /*public void PrimaryMusic(AudioClip musicClip, float volume)
    {
        AudioSource activeSource = musicSource;

        activeSource.clip = musicClip;
        activeSource.volume = volume;
        activeSource.Play();

    }
    public void SecondaryMusic(AudioClip musicClip, float volume)
    {
        AudioSource activeSource = musicSource2;

        activeSource.clip = musicClip;
        activeSource.volume = volume;
        activeSource.Play();

    }*/
}

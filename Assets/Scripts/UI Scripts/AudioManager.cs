using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void PlayMusic(AudioClip musicClip)
    {
        AudioSource activeSource = (musicSourcePlaying) ? musicSource : musicSource2;

        activeSource.clip = musicClip;
        activeSource.volume = 1;
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
        sfxSource.PlayOneShot(clip);
    }
    public void PlaySFX(AudioClip clip, float volume)
    {
        sfxSource.PlayOneShot(clip, volume);
    }


    // EXAMPLE OF HOW TO REFERNCE THE AUDIO CLIP
    /*private AudioSource leftMouse; //AudioSource will be created for left mouse click

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        leftMouse = this.gameObject.AddComponent<AudioSource>(); //AudioSource added to scene
    }

    public void lmSFX(AudioClip clip) //left mouse click
    {
        leftMouse.PlayOneShot(clip);
    }*/

}

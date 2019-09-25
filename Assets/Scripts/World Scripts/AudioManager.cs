using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
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

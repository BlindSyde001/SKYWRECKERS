﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioPlayPerScene : MonoBehaviour
{
    //VARIABLES
    private AudioManager _audioManager;
    public AudioClip clip;
    public float volume;

    //UPDATES
    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
    }
    private void Start()
    {
        _audioManager.PlayMusic(clip, volume);
    }

    //METHODS

}
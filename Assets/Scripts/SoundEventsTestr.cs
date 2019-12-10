using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEventsTestr : MonoBehaviour
{
    //VARIABLES
    public bool jumpcheck = false;
    public bool runcheck = false;
    public bool climbcheck = false;
    private AudioManager _audioManager;

    //UPDATES
    private void Update()
    {
        _audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
    }

    //METHODS
    public void Turnon()
    {
        _audioManager.PlaySFX(_audioManager.jump);
    }

    public void TurnonClimb()
    {
        _audioManager.PlaySFX(_audioManager.climb);
    }

}

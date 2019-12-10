using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Turnon()
    {
        _audioManager.PlaySFX(_audioManager.jump);
    }
    //public void Turnoff()
    //{
    //    print("Jump" + jumpcheck);
    //    jumpcheck = false;
    //}

    public void TurnonClimb()
    {
        _audioManager.PlaySFX(_audioManager.climb);
    }
    //public void TurnoffClimb()
    //{
    //    print("Climb" + climbcheck);
    //    climbcheck = false;
    //}

    //public void TurnonRun()
    //{
    //    print("Run" + jumpcheck);
    //    jumpcheck = true;
    //}
    //public void TurnoffRun()
    //{
    //    print("Run" + runcheck);
    //    runcheck = false;
    //}

}

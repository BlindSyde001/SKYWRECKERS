using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraControlsShip : MonoBehaviour
{
    //VARIABLES
    public GameObject thirdPersonCamera;
    public GameObject overShoulderCamera;

    AudioListener thirdPersonAudioLis;
    AudioListener overShoulderListener; 


    //UPDATES
    void Start()
    {
        thirdPersonAudioLis = thirdPersonCamera.GetComponent<AudioListener>();
        overShoulderListener = overShoulderCamera.GetComponent<AudioListener>();

        cameraPositionChange(PlayerPrefs.GetInt("CameraPosition"));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ChangeControls();
        }
    }

    void FixedUpdate()
    {
        switchCamera();
    }

    //METHODS
    void switchCamera()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            cameraChangeCounter();
        }
    }

    void cameraChangeCounter()
    {
        int cameraPositionCounter = PlayerPrefs.GetInt("CameraPosition");
        cameraPositionCounter++;
        cameraPositionChange(cameraPositionCounter);
    }

    void cameraPositionChange(int camPosition)
    {
        if (camPosition > 1)
        {
            camPosition = 0;
        }
        PlayerPrefs.SetInt("CameraPosition", camPosition);


        if (camPosition == 0)
        {
            overShoulderCamera.SetActive(true);
            overShoulderListener.enabled = true;

            thirdPersonAudioLis.enabled = false;
            thirdPersonCamera.SetActive(false);
        }

        if (camPosition == 1)
        {
            thirdPersonCamera.SetActive(true);
            thirdPersonAudioLis.enabled = true;

            overShoulderListener.enabled = false;
            overShoulderCamera.SetActive(false);
        }

    }

    void ChangeControls()
    {
        
    }
}

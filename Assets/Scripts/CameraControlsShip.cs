using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlsShip : MonoBehaviour
{
    public GameObject thirdPersonCamera;
    public GameObject overShoulderCamera;

    AudioListener thirdPersonAudioLis;
    AudioListener overShoulderListener; 


    // Start is called before the first frame update
    void Start()
    {
        thirdPersonAudioLis = thirdPersonCamera.GetComponent<AudioListener>();
        overShoulderListener = overShoulderCamera.GetComponent<AudioListener>();

        cameraPositionChange(PlayerPrefs.GetInt("CameraPosition"));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switchCamera();
    }

    public void cameraPositonM()
    {
        cameraChangeCounter();
    }

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
}

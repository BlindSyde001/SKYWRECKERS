﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraControls : MonoBehaviour
{
    //VARIABLES
    public Transform playerController;

    private Camera cam;

    public float distance = 2f;
    private float currentX = 0f;
    private float currentY = 0f;
    private const float yAngleMin = -45f;
    private const float yAngleMax = 45f;

    public Transform shoulderCam;
    public int changeCounter = 0;
    bool check;

    //UPDATES
    private void Start()
    {
        playerController = GameObject.Find("Player").transform;
        cam = Camera.main;
    }

    private void Update()
    {
        check = playerController.GetComponent<PlayerMovement>().isControllingShip;
       
        if(check)
        {
            currentX += Input.GetAxis("Mouse X");
            currentY += Input.GetAxis("Mouse Y");
            currentY = Mathf.Clamp(currentY, yAngleMin, yAngleMax);

        }
            

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (check)
            {
                CameraPosChange();
            }
        }
    }

    private void LateUpdate()
    {
        FreeRotateCamera();
    }

    //METHODS
    void FreeRotateCamera()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = playerController.position + rotation * dir;
        transform.LookAt(playerController.position);
    }

    void CameraPosChange()
    {
        changeCounter++;
        if (changeCounter > 1)
        {
            changeCounter = 0;
        }
        movePostion();
    }

    void movePostion()
    {
        switch (changeCounter)
        {
            case 0:
                distance = 0.5f;
                break;
            case 1:
                distance = 10;
                break;
        }
    }

    public void SwitchButton()
    {
        changeCounter = 0;
        cam.transform.position = shoulderCam.transform.position;

        movePostion();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShipTwo : MonoBehaviour
{
    //VARIABLES
    public Transform playerPos;
    public Transform camTransform;

    private Camera cam;

    public float distance = 2f;
    private float currentX = 0f;
    private float currentY = 0f;
    private const float yAngleMin = -45f;
    private const float yAngleMax = 45f;

    public Transform orbitalPos;
    public Transform overShoulderPos;
    public int changeCounter = 0;

    //UPDATES
    private void Start()
    {
        playerPos = GameObject.Find("Player").transform;
        cam = Camera.main;
    }

    private void Update()
    {
        currentX += Input.GetAxis("Mouse X");
        currentY += Input.GetAxis("Mouse Y");
        currentY = Mathf.Clamp(currentY, yAngleMin, yAngleMax);
        FreeRotateCamera();

        if (Input.GetKeyDown(KeyCode.V))
        {
            CameraPosChange();
        }
    }

    //METHODS
    void FreeRotateCamera()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = playerPos.position + rotation * dir;
        transform.LookAt(playerPos.position);
    }

    void CameraPosChange()
    {
        changeCounter++;
        if(changeCounter > 1)
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
                distance = 4;
                break;
            case 1:
                distance = 10;
                break;
        }
    }
}

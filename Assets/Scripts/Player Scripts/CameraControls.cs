using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraControls : MonoBehaviour
{
    //VARIABLES
    public Transform playerController;

    public Camera cam;

    private float targetDistance = 10f;
    private float currentDistance = 10f;
    public float cameraRelocate = 0.2f;
    private float currentX = 0f;
    private float currentY = 0f;
    private const float yAngleMin = -45f;
    private const float yAngleMax = 45f;

    public List<GameObject> shipClippingParts;
    public Transform shoulderCam;
    public int changeCounter = 0;
    bool check;

    //UPDATES
    private void Start()
    {
        playerController = GameObject.Find("Player").transform;
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

        movePostion();
    }

    private void LateUpdate()
    {
        FreeRotateCamera();
        CameraClipping();
    }

    //METHODS
    void FreeRotateCamera()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.rotation = rotation;
    }

    void CameraClipping()
    {
        Ray ray = new Ray(transform.position, -transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, targetDistance))
        {
            currentDistance = hit.distance;
            Debug.Log(hit.transform.name);
        }
        else
        {
            currentDistance = targetDistance;
        }
        cam.transform.localPosition = new Vector3(0, 0, -currentDistance + cameraRelocate);
        Debug.DrawRay(transform.position, -transform.forward * targetDistance);
    }

    void CameraPosChange()
    {
        changeCounter++;
        if (changeCounter > 1)
        {
            changeCounter = 0;
        }
    }

    void movePostion()
    {
        switch (changeCounter)
        {
            case 0:
                if(targetDistance != 5f)
                {
                     targetDistance -= 80f * Time.deltaTime;
                }
                if(targetDistance <= 5f)
                {
                   targetDistance = 5f;
                }
                foreach (GameObject x in shipClippingParts)
                {
                    x.layer = 0;
                }
                break;
            case 1:
                if (targetDistance != 80f)
                {
                    targetDistance += 80f * Time.deltaTime;
                }
                if (targetDistance >= 80f)
                {
                    targetDistance = 80f;
                }
                foreach(GameObject x in shipClippingParts)
                {
                    x.layer = 2;
                }
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

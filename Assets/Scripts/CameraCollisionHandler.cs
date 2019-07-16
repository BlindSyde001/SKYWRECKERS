using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionHandler : MonoBehaviour
{
    //VARIABLES

    public LayerMask collisionLayer;

    [HideInInspector]
    public bool colliding = false;
    [HideInInspector]
    public Vector3[] adjustedCameraClipPoints;
    [HideInInspector]
    public Vector3[] desiredCameraClipPoints;

    public CameraCollisionHandler collision = new CameraCollisionHandler();
    Camera camera;
    Vector3 destination = Vector3.zero;
    Vector3 adjustedDestination = Vector3.zero;
    Vector3 camVel = Vector3.zero;


    //UPDATES
    private void Start()
    {
        collision.Initialize(Camera.main);
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
        collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);
    }

    //METHODS
    public void Initialize(Camera cam)
    {
        camera = cam;
        adjustedCameraClipPoints = new Vector3[5];
        desiredCameraClipPoints = new Vector3[5];
    }
    public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
    {
        if (!camera)
        { return; }

        //clear array
        intoArray = new Vector3[5];

        float z = camera.nearClipPlane;
        float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
        float y = x / camera.aspect;

        //top left
        intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;
        //top right
        intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
        //bot left
        intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
        //bot right
        intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;
        //cam pos
        intoArray[4] = cameraPosition - camera.transform.forward;
    }
    bool CollisionDetectedClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
    {
        for(int i = 0; i < clipPoints.Length; i++)
        {
            Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
            float distance =  Vector3.Distance(clipPoints[i], fromPosition);

            if(Physics.Raycast(ray, distance, collisionLayer))
            {
                return true;
            }

        }
        return false;
    }
    
    public float GetAdjustedDistanceWithRayFrom(Vector3 from)
    {
        float distance = -1;

        for( int i = 0; i < desiredCameraClipPoints.Length; i++)
        {
            Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if(distance == -1)
                {
                    distance = hit.distance;
                }
                else
                {
                    if (hit.distance < distance)
                    {
                        distance = hit.distance;
                    }
                }

            }
        }
        if (distance == -1)
        {
            return 0;
        }
        else { return distance; }
    }
    public void CheckColliding(Vector3 targetPosition)
    {
        if(CollisionDetectedClipPoints(desiredCameraClipPoints, targetPosition))
        {
            colliding = true;
        } else
        {
            colliding = false;
        }
    }

}

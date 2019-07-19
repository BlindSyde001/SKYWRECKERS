using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionHandler : MonoBehaviour
{
    //VARIABLES

    [SerializeField]
    private Vector2 zoomClamp = new Vector2(0.5f, 25f);

    [SerializeField]
    private float zoomDamp = 0.05f;

    [SerializeField]
    private float cameraRadius = 0.5f;

    private float targetZoom = 5f, zoom = 5f;
    private float zoomVelocity = 0f;

    //UPDATES
    private void Update()
    {
        Zoom();
    }

    //METHODS
    private void Zoom()
    {
        targetZoom += Input.GetAxis("Mouse ScrollWheel");
        targetZoom = Mathf.Clamp(targetZoom, -zoomClamp.x, zoomClamp.y);

        Ray ray = new Ray(transform.parent.position, -transform.parent.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, zoom))
        {
            zoom = Mathf.SmoothDamp(zoom, hit.distance, ref zoomVelocity, zoomDamp);
            zoom -= cameraRadius;

        }else
        {
            zoom = Mathf.SmoothDamp(zoom, targetZoom, ref zoomVelocity, zoomDamp);
        }
        zoom = Mathf.Clamp(zoom, -zoomClamp.x, zoomClamp.y);
        transform.localPosition = Vector3.back * zoom;
    }
}

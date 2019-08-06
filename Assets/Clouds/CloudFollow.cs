using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudFollow : MonoBehaviour
{
    public Transform shipCentre;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 fixedShipPos = shipCentre.position;
        fixedShipPos.y = -50.5f; //Can change depending on the Y axis of the clouds (in the future try making the Y 0 and then moving everything else up
        transform.position = fixedShipPos;
    }
}

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
        fixedShipPos.y = 0;
        transform.position = fixedShipPos;
    }
}

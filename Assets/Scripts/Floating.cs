using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    public float rotationSpeed = 15.0f;
    public float bobRange = 1f;
    public float bobSpeed = 1f;
    
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    // Use this for initialization
    void Start()
    {
        posOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, Time.deltaTime * rotationSpeed, 0f), Space.World);
        
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * bobSpeed) * bobRange;

        transform.position = tempPos;
    }
}

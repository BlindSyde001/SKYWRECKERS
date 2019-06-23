using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovementControlsShip : MonoBehaviour
{
    //VARIABLES
    private Rigidbody shipRigidBody;
    public float topSpeed;
    public float zeroToMaxPerSecond;
    float acceleration;
    public float forwardVelocity;

    //UPDATES

    private void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();
        acceleration = topSpeed / zeroToMaxPerSecond;
        forwardVelocity = 0f;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            forwardVelocity += acceleration * Time.deltaTime;
        }
        
        forwardVelocity = Mathf.Min(forwardVelocity, topSpeed);
    }

    private void LateUpdate()
    {
        shipRigidBody.velocity = transform.forward * forwardVelocity;
    }

    //METHODS
}

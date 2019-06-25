using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovementControlsShip : MonoBehaviour
{
    //VARIABLES
    private Rigidbody shipRigidBody;
    public float zeroToMaxPerSecond;
    public float forwardVelocity;
    public int accelerateModeCounter = 0;
    private float speedVelocity;

    private Vector3 input;
    private Quaternion targetRotation;
    private float angle;
    public float pitchSpeed = 1;
    public float turnSpeed = 75F;
    private float yaw;

    public float transitionCooldown = 1F;
    private float transitionTimer = 0F;

    //UPDATES

    private void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();
        forwardVelocity = 0f;
        yaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        input.z = Input.GetAxisRaw("Cension");
        turnCap();
        calculateDirectionZ();
        rotation();
        if (transitionTimer <= 0F)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                accelerateModeCounter++;
                transitionTimer = transitionCooldown;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                accelerateModeCounter--;
                transitionTimer = transitionCooldown;
            }
        }
        else
        {
            transitionTimer -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            yaw -= turnSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            yaw += turnSpeed * Time.deltaTime;
        }

        accelerateModeCounter = Mathf.Clamp(accelerateModeCounter, 0, 3);
        forwardVelocity = Mathf.SmoothDamp(forwardVelocity, 10 * accelerateModeCounter, ref speedVelocity, zeroToMaxPerSecond);
    }

    private void LateUpdate()
    {
        shipRigidBody.velocity = transform.forward * forwardVelocity;
    }

    //METHODS

    void rotation()
    {
        targetRotation = Quaternion.Euler(angle, yaw, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, pitchSpeed * Time.deltaTime);
    }
    void calculateDirectionZ()
    {
        angle = Mathf.Atan(input.z);
        angle = Mathf.Rad2Deg * angle;
    }

    void turnCap()
    {
        turnSpeed = 100 - (accelerateModeCounter * 22);
        if(accelerateModeCounter == 0)
        {
            turnSpeed = 25;
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    //VARIABLES
    public float velocity = 5f;
    public float turnSpeed = 10;
    public float jumpSpeed = 3.5f;

    public float distance = 2f;
    private float currentX = 0f;
    private float currentY = 0f;
    private const float yAngleMin = -45f;
    private const float yAngleMax = 45f;
    private Transform wallClimb;

    [NonSerialized]
    public bool isControllingShip = true;
    public bool isClimbing = false;
    public GameObject shipHP;
    CharacterController controller;

    public new Camera camera;
    public GameObject shoulderPos;

    public Transform dockPos;
    public MovementControlsShip ship;
    float shoulderRot;

    private bool shipGrounded;
    public GameObject Bedroom;
    public float threshold;
    private float yVelocity;
    private Vector3 movement;

    //New MOVEMENT variables
    public float fowardMovement = 2.5f;
    public float backMovement = 2.25f;
    public float strafeMovement = 2f;
    public float slowMod = 0.5f;
    private float slowMovement;

    //UPDATES
    private void Awake()
    {
        ship = FindObjectOfType<MovementControlsShip>();
        controller = GetComponent<CharacterController>();

        camera.gameObject.SetActive(!isControllingShip);
        ship.camera.gameObject.SetActive(isControllingShip);
    }

    private void Update()
    {
        if (!isControllingShip)
        {
            Move();
            ClimbingControls();
            Rotation();
            playerJumping();

            if(!shipGrounded)
            shipHP.SetActive(false);
        }
        else
        {
            transform.position = dockPos.position;
            shipHP.SetActive(true);
        }
    }
    public void FixedUpdate()
    {
        if (transform.position.y < threshold)
            transform.position = Bedroom.transform.position;
    }

    //METHODS

    void playerJumping()
    {
        if (!isClimbing && controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            yVelocity = jumpSpeed;
        }

    }

    void Move()
    {
        if(!isClimbing)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                slowMovement = slowMod;
            }
            else
            {
                slowMovement = 1.0f;
            }

            if (Input.GetKey(KeyCode.W))
            {
                movement += transform.forward * (fowardMovement * slowMovement);
            }
            if (Input.GetKey(KeyCode.S))
            {
                movement += -transform.forward * (backMovement * slowMovement);
            }
            if (Input.GetKey(KeyCode.A))
            {
                movement += -transform.right * (strafeMovement * slowMovement);
            }
            if (Input.GetKey(KeyCode.D))
            {
                movement += transform.right * (strafeMovement * slowMovement);
            }
            yVelocity += Physics.gravity.y * Time.deltaTime;
        }else
        {
            yVelocity = 0;
        }
        
        //controller.Move((yVelocity * Vector3.up + movement + (shipGrounded ? ship.displacement : Vector3.zero)) * Time.deltaTime);
        //ship.displacement = Vector3.zero;
        //movement = Vector3.zero;

        Vector3 newMove = Vector3.Lerp(controller.velocity, movement, Time.deltaTime * 5f);
        ship.displacement = Vector3.zero;
        controller.Move((yVelocity * Vector3.up + newMove + (shipGrounded ? ship.displacement : Vector3.zero)) * Time.deltaTime);
        print(controller.velocity);
        print("MOVEMENT: " + movement);
        movement = Vector3.zero;

        if (controller.isGrounded)
            yVelocity = 0F;
    }

    private void Rotation()
    {
        
        
            float horizontal = Input.GetAxis("Mouse X");

            transform.Rotate(new Vector3(0F, horizontal * turnSpeed, 0F) * Time.deltaTime);

            float vertical = Input.GetAxis("Mouse Y");
            shoulderRot += vertical;
            shoulderRot = Mathf.Clamp(shoulderRot, -20, 20);

            shoulderPos.transform.localEulerAngles = new Vector3(shoulderRot, transform.rotation.y, 0);
        
    }

    #region MAGIC E
    //CLIMBING - Testing placing climbing mechanic on player. Need to make the bool a toggle so that player can attach and detach freely.
    public void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(other.CompareTag("Wheel") && !ship.docking)
            {
                isControllingShip = !isControllingShip;
                camera.gameObject.SetActive(!isControllingShip);
                ship.camera.gameObject.SetActive(isControllingShip);
            }

            if (other.CompareTag("Wall"))
            {
                isClimbing = !isClimbing;
                wallClimb = other.transform;
            }
        }
        if(other.CompareTag("ShipGround"))
        {
            shipGrounded = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            isClimbing = false;
        }
        if(other.CompareTag("ShipGround"))
        {
            shipGrounded = false;
        }
        

    }

    public void ClimbingControls()
    {
        switch (isClimbing)
        {
            case true:
                if (Input.GetKey(KeyCode.W))
                {
                    movement += wallClimb.up * 2F;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    movement += -wallClimb.right * 2F;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    movement += -wallClimb.up * 2F;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    movement += wallClimb.right * 2F;
                }
                break;
        }
    }
    #endregion
}

using System;
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

    [NonSerialized]
    public bool isControllingShip = true;
    public bool isClimbing = false;
    //Rigidbody rb;
    CharacterController controller;

    public new Camera camera;
    public GameObject shoulderPos;

    public Transform dockPos;
    private MovementControlsShip ship;
    float shoulderRot;

    private bool shipGrounded;

    private float yVelocity;
    private Vector3 movement;

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
        }
        else
        {
            transform.position = dockPos.position;
            
        }
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
            if (Input.GetKey(KeyCode.W))
            {
                movement += transform.forward * 5F;

                //transform.Translate((Vector3.forward * 5) * Time.deltaTime, Space.Self);
            }
            if (Input.GetKey(KeyCode.S))
            {
                movement += -transform.forward * 5F;

                //transform.Translate((Vector3.back * 5) * Time.deltaTime, Space.Self);
            }
            if (Input.GetKey(KeyCode.A))
            {
                movement += -transform.right * 5F;

                //transform.Translate((Vector3.left * 5) * Time.deltaTime, Space.Self);
            }
            if (Input.GetKey(KeyCode.D))
            {
                movement += transform.right * 5F;

                //transform.Translate((Vector3.right * 5) * Time.deltaTime, Space.Self);
            }
            yVelocity += Physics.gravity.y * Time.deltaTime;
        }else
        {
            yVelocity = 0;
        }
        
        controller.Move((yVelocity * Vector3.up + movement + (shipGrounded ? ship.displacement : Vector3.zero)) * Time.deltaTime);
        ship.displacement = Vector3.zero;
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
                //rb.useGravity = !isControllingShip;
                camera.gameObject.SetActive(!isControllingShip);
                ship.camera.gameObject.SetActive(isControllingShip);
            }

            if (other.CompareTag("Wall"))
            {
                isClimbing = !isClimbing;
                //rb.useGravity = false;
            }
        }
        if(other.CompareTag("ShipGround"))
        {
            //transform.parent = other.transform;
            shipGrounded = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            isClimbing = false;
            //rb.useGravity = true;
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
                    movement += transform.up * 2F;
                    //transform.Translate((Vector3.up * 5) * Time.deltaTime, Space.World);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    movement += -transform.right * 2F;
                    //transform.Translate((Vector3.left * 5) * Time.deltaTime, Space.World);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    movement += -transform.up * 2F;
                    //transform.Translate((Vector3.down * 5) * Time.deltaTime, Space.World);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    movement += transform.right * 2F;
                    //transform.Translate((Vector3.right * 5) * Time.deltaTime, Space.World);
                }
                break;
        }
    }
    #endregion
}

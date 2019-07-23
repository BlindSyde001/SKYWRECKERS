using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    //VARIABLES
    public float velocity = 5f;
    public float turnSpeed = 10;
    public float jumpSpeed = 150f;

    public float distance = 2f;
    private float currentX = 0f;
    private float currentY = 0f;
    private const float yAngleMin = -45f;
    private const float yAngleMax = 45f;

    [NonSerialized]
    public bool isControllingShip = true;
    public bool isClimbing = false;
    Rigidbody rb;

    public new Camera camera;
    public GameObject shoulderPos;

    public Transform dockPos;
    private MovementControlsShip ship;
    float shoulderRot;

    //PlayerJumping
    public bool isGrounded; //Checks if player is colliding with ground

    //UPDATES
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ship = FindObjectOfType<MovementControlsShip>();

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
            gameObject.transform.position = dockPos.position;
        }
    }

    //METHODS

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == ("Ground") && isGrounded == false)
        {
            isGrounded = true;
            playerJumping();
        }
    }

    void playerJumping()
    {
        if (!isClimbing && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false;
            rb.AddForce(Vector3.up * jumpSpeed);
        }

    }

    void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
           transform.Translate((Vector3.forward * 5) * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate((Vector3.back * 5) * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate((Vector3.left * 5) * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate((Vector3.right * 5) * Time.deltaTime, Space.Self);
        }

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
            if(other.CompareTag("Wheel"))
            isControllingShip = !isControllingShip;
            rb.useGravity = !isControllingShip;

            camera.gameObject.SetActive(!isControllingShip);
            ship.camera.gameObject.SetActive(isControllingShip);
        }
        if (Input.GetKeyDown(KeyCode.L) && other.gameObject.tag == "Wall")
        {
            
            isClimbing = true;
            rb.useGravity = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        isClimbing = false;
        rb.useGravity = true;
    }
    // Very quick and dirty climbing controls. Designed for simple functionality, not for final build.
    //Currently does not work, as standard controls interfere with climbing.
    public void ClimbingControls()
    {
        switch (isClimbing)
        {
            case true:
                if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate((Vector3.up * 5) * Time.deltaTime, Space.World);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Translate((Vector3.left * 5) * Time.deltaTime, Space.World);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate((Vector3.down * 5) * Time.deltaTime, Space.World);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.Translate((Vector3.right * 5) * Time.deltaTime, Space.World);
                }
                break;
        }
    }
    #endregion
}

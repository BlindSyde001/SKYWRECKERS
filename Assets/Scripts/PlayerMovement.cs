using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    //VARIABLES
    public float velocity = 5f;
    public float turnSpeed = 10;

    [NonSerialized]
    public bool isControllingShip = true;
    public bool isClimbing = false; 

    private Vector2 input;
    private float angle;
    private Quaternion targetRotation;
    private Transform cam;
    Rigidbody rb;

    public Transform dockPos;
   
    //UPDATES
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
       
        if (!isControllingShip)
        {
            getInput();

            if (Mathf.Abs(input.x) < 1 && Mathf.Abs(input.y) < 1) return;
            {
                calculateDirection();
                rotate();
                Move();
                ClimbingControls();
                
            }
        } else if(isControllingShip)
        {
            this.gameObject.transform.position = dockPos.position;
            if(Input.GetKeyDown(KeyCode.E))
                {
                    isControllingShip = false;
                    rb.useGravity = true;
                Camera.main.SendMessage("SwitchButton");
                }
        }
    }

    //METHODS
    void getInput()
    {
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }
    }

   
    void calculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;
    }
   
    void rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
    void Move()
    {
        if (!isClimbing) 
        transform.position += transform.forward * velocity * Time.deltaTime;
    }

    //CLIMBING - Testing placing climbing mechanic on player. Need to make the bool a toggle so that player can attach and detach freely.
    public void OnTriggerStay(Collider other)
    {
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
                    print("UP");
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
}

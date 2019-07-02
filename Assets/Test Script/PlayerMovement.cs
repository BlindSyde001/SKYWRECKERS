﻿using System;
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
                
            }
        } else if(isControllingShip)
        {
            this.gameObject.transform.position = dockPos.position;
            if(Input.GetKeyDown(KeyCode.E))
                {
                    isControllingShip = false;
                    rb.useGravity = true;
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
        transform.position += transform.forward * velocity * Time.deltaTime;
    }

}

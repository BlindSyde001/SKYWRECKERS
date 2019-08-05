using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MovementControlsShip : MonoBehaviour
{
    //VARIABLES
    public float zeroToMaxPerSecond;
    public float forwardVelocity;
    public int accelerateModeCounter = 0;
    private float speedVelocity;

    public Vector3 input;
    private Quaternion targetRotation;
    private float angle;
    public float pitchSpeed = 1;
    public float turnSpeed = 75F;
    private float yaw;
    public float speed = 0;

    public float dockTime = 5F;

    public float transitionCooldown = 1F;
    private float transitionTimer = 0F;

    public List<Transform> bulletEndsRight;
    public List<Transform> bulletEndsLeft;
    int number;
    public Rigidbody bullet;
    public Transform barrelEnd;
    public float fireRate = 1f;
    float nextFire;

    private PlayerMovement player;
    public new Camera camera;
    public bool docking = false;

    float currentY;
    public Transform dockingPlacement;

    private CharacterController controller;
    private Vector3 movement;
    public Vector3 nudgeVector = Vector3.zero;

    [NonSerialized]
    public Vector3 displacement;

    //UPDATES

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        forwardVelocity = 0f;
        yaw = transform.eulerAngles.y;

        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
            if (!docking)
        {
            controller.enabled = true;

            if (!(player?.isControllingShip ?? true))
                return;

            #region Movement
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

            if (player?.isControllingShip ?? true)
            {
                turnCap();
                ShootingCannons();
                if (accelerateModeCounter > 0)
                {
                    input.z = Input.GetAxis("Cension");
                    calculateDirectionZ();
                }
            }
            else
            {
               //GetComponent<Rigidbody>().isKinematic = true;
            }

            accelerateModeCounter = Mathf.Clamp(accelerateModeCounter, 0, 3);
            forwardVelocity = Mathf.SmoothDamp(forwardVelocity, speed, ref speedVelocity, zeroToMaxPerSecond);
            if (forwardVelocity < 0.01f)
            {
                forwardVelocity = 0;
            }

            if (forwardVelocity > 0.01)
            {
                if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    yaw -= turnSpeed * Time.deltaTime;
                }

                if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
                {
                    yaw += turnSpeed * Time.deltaTime;
                }
                rotation();
            }
            #endregion
        }
        else
        {
            StartCoroutine(Dock());
        }
        
    }

    private IEnumerator Dock()
    {
        docking = true;

        float time = 0F;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while(time <= dockTime)
        {
            time += Time.deltaTime;

            controller.enabled = false;
            transform.position = Vector3.Lerp(startPos, dockingPlacement.position, time / dockTime);
            transform.rotation = Quaternion.Lerp(startRot, dockingPlacement.rotation, time / dockTime);
            forwardVelocity = 0;
            controller.enabled = true;

            yield return null;
        }

        yaw = transform.eulerAngles.y;
        docking = false;
    }

    private void LateUpdate()
    {
        controller.Move((nudgeVector + movement + transform.forward * forwardVelocity) * Time.deltaTime);

        if(player.isControllingShip)
           player.transform.position = player.dockPos.position;


        movement = Vector3.zero;
        nudgeVector = Vector3.Lerp(nudgeVector, Vector3.zero, 2f * Time.deltaTime);
        displacement = controller.velocity;
    }

    //METHODS   

    void rotation()
    {
        if(accelerateModeCounter == 0)
        {
            targetRotation = Quaternion.Euler(0, yaw, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, pitchSpeed/2 * Time.deltaTime);
        } else
        {
            targetRotation = Quaternion.Euler(angle / 2, yaw, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, pitchSpeed * Time.deltaTime);
        }
    }
    void calculateDirectionZ()
    {
        angle = Mathf.Atan(input.z);
        angle = Mathf.Rad2Deg * angle;
    }

    void turnCap()
    {
        switch(accelerateModeCounter)
        {
            case 0:
                turnSpeed = 25;
                speed = 0;
                break;

            case 1:
                turnSpeed = 50;
                speed = 5;
                break;

            case 2:
                turnSpeed = 45;
                speed = 10;
                break;

            case 3:
                turnSpeed = 30;
                speed = 20;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Dock"))
        {
            if(Input.GetKeyDown(KeyCode.G) && !docking)
            {
                print("PRINT");
                if (!docking)
                {
                    docking = true;
                    dockingPlacement = other.GetComponent<Dock>().dockingPlacement;
                    accelerateModeCounter = 0;

                }
                else
                {
                    docking = false;
                }
                
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        yaw += 180F;
    }

    #region Cannons
    void ShootingCannons()
    {
        if (Input.GetMouseButton(1) && Time.time > nextFire)
        {

            nextFire = Time.time + fireRate;
            Rigidbody bulletInstance;
            foreach (Transform x in bulletEndsRight)
            {
                bulletInstance = Instantiate(bullet, x.position, x.rotation) as Rigidbody;
                bulletInstance.AddForce(barrelEnd.forward * 4500);
            }
        }
        else if (Input.GetMouseButton(0) && Time.time > nextFire)
        {

            nextFire = Time.time + fireRate;
            Rigidbody bulletInstance;
            foreach (Transform y in bulletEndsLeft)
            {
                bulletInstance = Instantiate(bullet, y.position, y.rotation) as Rigidbody;
                bulletInstance.AddForce(barrelEnd.forward * -4500);
            }
        }
    }
    #endregion
}


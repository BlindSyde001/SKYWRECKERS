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

    public Vector3 input;
    private Quaternion targetRotation;
    private float angle;
    public float pitchSpeed = 1;
    public float turnSpeed = 75F;
    private float yaw;

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

    float currentY;

    //UPDATES

    private void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();
        forwardVelocity = 0f;
        yaw = transform.eulerAngles.y;

        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        
        if (transitionTimer <= 0F)
        {
            if (player?.isControllingShip ?? true)
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
        }
        else
        {
            transitionTimer -= Time.deltaTime;
        }

        if (player?.isControllingShip ?? true)
        {
            turnCap();
            ShootingCannons();
            if(accelerateModeCounter > 0)
            {
                input.z = Input.GetAxis("Cension");
                calculateDirectionZ();

                if (GetComponent<Rigidbody>().isKinematic == true)
                {
                    GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        } else
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }

        accelerateModeCounter = Mathf.Clamp(accelerateModeCounter, 0, 3);
        forwardVelocity = Mathf.SmoothDamp(forwardVelocity, 10 * accelerateModeCounter, ref speedVelocity, zeroToMaxPerSecond);
        if (forwardVelocity < 0.01f)
        {
            forwardVelocity = 0;
        }

        if(forwardVelocity > 0.01)
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
    }

    private void LateUpdate()
    {
        shipRigidBody.velocity = transform.forward * forwardVelocity;
    }

    //METHODS

    void rotation()
    {
        targetRotation = Quaternion.Euler(angle/2, yaw, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, pitchSpeed * Time.deltaTime);
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
                turnSpeed = 15;
                break;

            case 1:
                turnSpeed = 30;
                break;

            case 2:
                turnSpeed = 25;
                break;

            case 3:
                turnSpeed = 20;
                break;
        }
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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //VARIABLES
    private UIManager UI;

    public float velocity = 5f;
    public float turnSpeed = 10;
    public float jumpSpeed = 7f;

    public float distance = 2f;
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

    public bool shipGrounded;
    public GameObject Bedroom;
    public float threshold;
    private float yVelocity;
    private Vector3 movement;
    public float gravityScale;

    //New MOVEMENT variables
    public float fowardMovement = 2.5f; //Each movement variable is public so it can be changed when animations are imported
    public float backMovement = 2.25f;
    public float strafeMovement = 2f;
    public float slowMod = 0.5f;
    private float slowMovement;

    public GameObject plank;
    public GameObject gameover;
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
            plank.SetActive(true);
            if(!shipGrounded)
            shipHP.SetActive(false);
        }
        else
        {
            transform.position = dockPos.position;
            shipHP.SetActive(true);
            plank.SetActive(false);
        }
    }
    public void FixedUpdate()
    {
        if (transform.position.y < threshold)
        {
            Time.timeScale = 0;
            gameover.SetActive(true);
            Cursor.visible = true;
            if (Time.timeScale == 0)
            {
                this.GetComponent<PlayerMovement>().enabled = false;
            }

        }
            //transform.position = Bedroom.transform.position;
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
                slowMovement = slowMod; //When player is holding shift movement is slowed down on each axis and they walk
            }
            else
            {
                slowMovement = 1.0f; //When player isn't holding shift slow movement is reset
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
            yVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
        }else
        {
            yVelocity = 0;
        }
        
        controller.Move((yVelocity * Vector3.up + movement + (shipGrounded ? ship.displacement : Vector3.zero)) * Time.deltaTime);
        ship.displacement = Vector3.zero;
        movement = Vector3.zero;

        //Vector3 newMove = Vector3.Lerp(controller.velocity, movement, Time.deltaTime * 5f);
        ship.displacement = Vector3.zero;
        //controller.Move((yVelocity * Vector3.up + newMove + (shipGrounded ? ship.displacement : Vector3.zero)) * Time.deltaTime);
       // print(controller.velocity);
      //  print("MOVEMENT: " + movement);
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

        if (other.CompareTag("ShipGround"))
        {
            shipGrounded = true;
        }

        #region REPAIRING SHIP
        if (Input.GetKey(KeyCode.E))
        {
            if(other.CompareTag("RepairPoint"))
            {
                other.GetComponent<Repair>().on = true;
            }
        }else if (Input.GetKeyUp(KeyCode.E))
        {
            if (other.CompareTag("RepairPoint"))
            {
                other.GetComponent<Repair>().on = false;
                other.GetComponent<Repair>().f = 0;
            }
        }
        #endregion
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
        if(other.CompareTag("RepairPoint"))
        {
            other.GetComponent<Repair>().on = false;
            other.GetComponent<Repair>().f = 0;
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

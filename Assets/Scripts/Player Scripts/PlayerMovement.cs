using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DiasGames.ThirdPersonSystem;
using DiasGames.ThirdPersonSystem.Cameras;

public class PlayerMovement : MonoBehaviour
{

    //VARIABLES
    private GameManager gm;
    private AudioManager _audioManager;
    public CameraController ccFinal;
    private UIManager ui;
    public CameraData cD;

    public GameObject menu;
    public GameObject overMenu;
    public GameObject controlsPanel;
    public GameObject settingsPanel;
    public GameObject savePanel;
    public GameObject inventoryPanel;
    public GameObject map;

    public float velocity = 5f;
    public float turnSpeed = 10;
    public float jumpSpeed = 7f;

    public float distance = 2f;
    private const float yAngleMin = -45f;
    private const float yAngleMax = 45f;
    private Transform wallClimb;
    public bool isControllingShip = false;
    public bool accessingMap = false;

    [NonSerialized]
    public bool isClimbing = false;
    public GameObject shipHP;

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
    public Button Settings;

    public bool inverted = false;

    public List<GameObject> dockingIcons;
    
    //UPDATES
    private void Awake()
    {
        ship = FindObjectOfType<MovementControlsShip>();
        gm = FindObjectOfType<GameManager>();
        ccFinal = FindObjectOfType<CameraController>();
        ui = FindObjectOfType<UIManager>();
        _audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
        Cursor.visible = false;
    }
    private void Start()
    {
        transform.position = gm.lastCheckpointPos.transform.position;
        StartCoroutine(ui.UiImage(ui.playerControls));
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            MenuActivate();
        }
        if (!isControllingShip)
        {
            plank.SetActive(true);
            if(!shipGrounded)
            shipHP.SetActive(false);
            if(!accessingMap)
            {
               this.GetComponent<UnityInputManager>().enabled = true;
            }
            ccFinal.m_UpdateType = AbstractFollowerCamera.UpdateType.FixedUpdate;
        }
        else
        {
            transform.position = dockPos.position;
            transform.rotation = dockPos.rotation;
            shipHP.SetActive(true);
            plank.SetActive(false);
            this.GetComponent<UnityInputManager>().enabled = false;
            ccFinal.m_UpdateType = AbstractFollowerCamera.UpdateType.LateUpdate;
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
    }
    private void LateUpdate()
    {
        if(isControllingShip)
        {
            if(Input.GetKeyDown(KeyCode.V) && cD.Offset.z == -1.5f)
            {
                cD.Offset.z -= 40;
            } else if(Input.GetKeyDown(KeyCode.V) && cD.Offset.z == -41.5f)
            {
                cD.Offset.z += 40;
            }
        } else
        {
            cD.Offset.z = -1.5f;
        }
    }

    //METHODS

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
        
        ship.displacement = Vector3.zero;
        movement = Vector3.zero;
        
        ship.displacement = Vector3.zero;
        movement = Vector3.zero;
    }

    private void Rotation()
    {
            float horizontal = Input.GetAxis("Mouse X");

            transform.Rotate(new Vector3(0F, horizontal * turnSpeed, 0F) * Time.deltaTime);

            float vertical = Input.GetAxis("Mouse Y");
        if(inverted)
        {
            shoulderRot += vertical;
        } else { shoulderRot -= vertical; }
            
            shoulderRot = Mathf.Clamp(shoulderRot, -20, 20);

            shoulderPos.transform.localEulerAngles = new Vector3(shoulderRot, transform.rotation.y, 0);
        
    }

    #region MAGIC E
    //CLIMBING - Testing placing climbing mechanic on player. Need to make the bool a toggle so that player can attach and detach freely.
    public void OnTriggerStay(Collider other)
    {
        bool can = false;
        if (ship.angle >= -3 && ship.angle <= 3)
        {
            can = true;
        }
        else
        {
            can = false;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            print("E");
            if(other.CompareTag("Wheel") && !ship.docking)
            {
                if(can)
                {
                 isControllingShip = !isControllingShip;
                    if(isControllingShip)
                    {
                        ui.StartCoroutine(ui.UiImage(ui.shipControls));
                        ui.UIObjectText.text = "";
                        _audioManager.PlayMusicWithFade(_audioManager.exploration, 5f);
                        foreach (GameObject dI in dockingIcons)
                        {
                            if (dI.activeSelf == false)
                            {
                                dI.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        ui.StartCoroutine(ui.UiImage(ui.playerControls));
                        _audioManager.PlayMusicWithFade(_audioManager.wind, 5f);
                        foreach (GameObject dI in dockingIcons)
                        {
                            if (dI.activeSelf == true)
                            {
                                dI.SetActive(false);
                            }
                        }
                    }
                }
            }
            if (other.CompareTag("Map Table"))
            {
                print("map");
                map.SetActive(!map.activeSelf);

                if(map.activeSelf == true)
                {
                    accessingMap = true;
                    this.GetComponent<UnityInputManager>().enabled = false;
                } else
                {
                    accessingMap = false;
                }

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

        if(other.CompareTag("Wheel") && !isControllingShip)
        {
            ui.UIObjectText.text = "Press E to take the Wheel";
        } 

        if(other.CompareTag("Map Table"))
        {
            ui.UIObjectText.text = "Press E to Access the Map";
        }

        #region REPAIRING SHIP
        if (Input.GetKey(KeyCode.E))
        {
            if(other.CompareTag("RepairPoint"))
            {
                other.GetComponent<Repair>().on = true;
                if(other.GetComponent<Repair>().healthSlider.gameObject.activeSelf == false)
                {
                    other.GetComponent<Repair>().healthSlider.gameObject.SetActive(true);
                }
            }
        }else if (Input.GetKeyUp(KeyCode.E))
        {
            if (other.CompareTag("RepairPoint"))
            {
                other.GetComponent<Repair>().on = false;
                other.GetComponent<Repair>().f = 0;
                if (other.GetComponent<Repair>().healthSlider.gameObject.activeSelf == true)
                {
                    other.GetComponent<Repair>().healthSlider.gameObject.SetActive(false);
                }
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
            print("EXITED");
            other.GetComponent<Repair>().on = false;
            other.GetComponent<Repair>().f = 0;
            if (other.GetComponent<Repair>().healthSlider.gameObject.activeSelf == true)
            {
                other.GetComponent<Repair>().healthSlider.gameObject.SetActive(false);
            }
        }
        if(other.CompareTag("Wheel"))
        {
            ui.UIObjectText.text = "";
        }

        if(other.CompareTag("Map Table"))
        {
            ui.UIObjectText.text = "";
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

    public void invertControls()
    {
        inverted = true;
    }
    public void normalControls()
    {
        inverted = false;
    }
    public void MenuActivate()
    {
        overMenu.SetActive(true);
        inventoryPanel.SetActive(false);
        savePanel.SetActive(false);
        controlsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        menu.SetActive(!menu.activeSelf);

        if(menu.activeSelf == true)
        {
            Settings.Select();
            Cursor.visible = true;
        } else
        {
            Cursor.visible = false;
        }
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void BackFromMap()
    {
        print("map");
        map.SetActive(!map.activeSelf);

        if (map.activeSelf == true)
        {
            accessingMap = true;
            this.GetComponent<UnityInputManager>().enabled = false;
        }
        else
        {
            accessingMap = false;
        }
    }
}

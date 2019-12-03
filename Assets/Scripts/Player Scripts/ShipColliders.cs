using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipColliders : MonoBehaviour
{
    //VARIABLES
    private UIManager UI;
    public UIManager hitPos;
    MovementControlsShip ship;

    public bool left = false;
    public bool right = false;
    public bool hull = false;
    public bool sail = false;


    public int damage;
    public bool dot = false;

    float timer = 3f;
    float timerTick = 0f;
    int dotTicks = 0;

    public Transform frontCheck;
    public Transform backCheck;
    private Vector3 collisionPoint;

    private Vector3 _direction;
    private Quaternion _lookRotation;

    //UPDATES
    private void Awake()
    {
        UI = FindObjectOfType<UIManager>();
        ship = FindObjectOfType<MovementControlsShip>();
    }

    private void Update()
    {
        if (dot)
        {
            DOTDamage();
        }
    }

    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if (ship.docking == false)
        {
            #region Colliding DPS
            if (other.CompareTag("Enemy") || other.CompareTag("Ground"))
            {
                if (sail)
                {
                    UI.GetComponent<UIManager>().sailsHP -= 15;
                    if(other.GetComponentInParent<PirateShipAI>() == null)
                    { 
                    Vector3 direction = (transform.position - other.transform.position).normalized;
                    ship.nudgeVector = direction * 80f;
                    }
                    Debug.Log(other);
                }

                if (hull)
                {
                    UI.GetComponent<UIManager>().hullHP -= 15;
                    if (other.GetComponentInParent<PirateShipAI>() == null)
                    {
                        Vector3 direction = (transform.position - other.transform.position).normalized;
                        ship.nudgeVector = direction * 80f;
                    }
                    Debug.Log(other);
                }

                if (left)
                {
                    UI.GetComponent<UIManager>().leftCannonHP -= 15;
                    if (other.GetComponentInParent<PirateShipAI>() == null)
                    {
                        Vector3 direction = (transform.position - other.transform.position).normalized;
                        ship.nudgeVector = direction * 80f;
                    }
                    Debug.Log(other);
                }

                if (right)
                {
                    UI.GetComponent<UIManager>().rightCannonHP -= 15;
                    if (other.GetComponentInParent<PirateShipAI>() == null)
                    {
                        Vector3 direction = (transform.position - other.transform.position).normalized;
                        ship.nudgeVector = direction * 80f;
                    }
                    Debug.Log(other);
                }
            }
            #endregion
            #region Pirate Ship Collision
            if(other.GetComponentInParent<PirateShipAI>() != null)
            {
                print("Its Working");
                collisionPoint = other.transform.position;
            }
            #endregion
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponentInParent<PirateShipAI>() != null)
        {
             PirateShipAI pS = other.GetComponentInParent<PirateShipAI>();

            if(pS.piercingBlowCheck)
            {
                MovementControlsShip ship = transform.GetComponentInParent<MovementControlsShip>();
                if (Vector3.Distance(collisionPoint, frontCheck.position) < Vector3.Distance(collisionPoint, backCheck.position))
                {
                    print("Front Check");
                    _direction = (ship.transform.position - other.transform.forward).normalized;
                    _lookRotation = Quaternion.LookRotation(_direction);

                    ship.transform.rotation = Quaternion.RotateTowards(transform.rotation, other.transform.rotation, 20 * Time.deltaTime);
                    ship.gettingRammed = true;
                }
                else if (Vector3.Distance(collisionPoint, frontCheck.position) > Vector3.Distance(collisionPoint, backCheck.position))
                {
                    print("Back Check");
                    _direction = (ship.transform.position - other.transform.forward).normalized;
                    _lookRotation = Quaternion.LookRotation(-_direction);

                    ship.transform.rotation = Quaternion.RotateTowards(transform.rotation, other.transform.rotation, 20 * Time.deltaTime);
                    ship.gettingRammed = true;
                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponentInParent<PirateShipAI>() != null)
        {
            transform.GetComponentInParent<MovementControlsShip>().gettingRammed = false;
        }
    }

    public void DamageTaken()
    {
        if (sail)
        {
            UI.GetComponent<UIManager>().sailsHP -= damage;
            Debug.Log("DT HIT SAIL");
        }


        if (hull)
        {
            UI.GetComponent<UIManager>().hullHP -= damage;
            Debug.Log("DT HIT HULL");
        }


        if (left)
        {
            UI.GetComponent<UIManager>().leftCannonHP -= damage;
            Debug.Log("DT HIT LEFT");
        }


        if (right)
        {
            UI.GetComponent<UIManager>().rightCannonHP -= damage;
            Debug.Log("DT HIT RIGHT");
        }
    }
    public void DOTDamage()
    {
        timerTick += Time.deltaTime;
        if (dotTicks >= 3)
        {
            dot = false;
        }
        if (timerTick >= timer)
        {
            DamageTaken();
            dotTicks++;
            timerTick = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShipAI : EnemyStats
{

    //VARIABLES
    private GameManager gm;

    private Vector3 _direction;
    private Transform playerShip;
    private Quaternion _lookRotation;
    private float rotateSpeed = 1.2f;
    public float moveSpeed = 10f;

    public float lookRadius = 120;
    private Transform alignPoint;

    public Transform playerShipLeft;
    public Transform playerShipRight;

    public List<Transform> bulletEndsRight;
    public List<Transform> bulletEndsLeft;
    public Rigidbody bullet;
    public Transform barrelEnd;
    public float fireRate = 1f;
    float nextFire;
    //UPDATES

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        playerShip = FindObjectOfType<MovementControlsShip>().transform;
        enemyMaxHP = 100;
        enemyCurrentHP = 100;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, playerShip.position) < lookRadius)
        {
            comeAlongSideShip();
        }
        if (enemyCurrentHP <= 0)
        {
            gm.enemyList.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }
    //METHODS

    void comeAlongSideShip()
    {
        //check for which side the ship is on relative to the player and come along side the player on that side
        float leftDistance = Vector3.Distance(transform.position, playerShipLeft.position);
        float rightDistance = Vector3.Distance(transform.position, playerShipRight.position);

        if(leftDistance < rightDistance)
        {
            alignPoint = playerShipLeft;
        }
            
        else
        {
            alignPoint = playerShipRight;
        }
         
        if(Vector3.Distance(transform.position, alignPoint.position) > 4)
        {
        _direction = (alignPoint.position - transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, rotateSpeed * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        } else
        {
            transform.position = Vector3.MoveTowards(transform.position, alignPoint.position, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, alignPoint.rotation, rotateSpeed  * Time.deltaTime);
            if(Vector3.Distance(transform.position, alignPoint.position) < 2)
            {
                ShootingCannons();
            }
        }
        print(alignPoint);
    }

    #region Cannons
    void ShootingCannons()
    {
        if (Time.time > nextFire && alignPoint == playerShipLeft)
        {

            nextFire = Time.time + fireRate;
            Rigidbody bulletInstance;
            foreach (Transform x in bulletEndsRight)
            {
                bulletInstance = Instantiate(bullet, x.position, x.rotation) as Rigidbody;
                bulletInstance.AddForce(barrelEnd.forward * 4500);
            }
        }
        else if (Time.time > nextFire && alignPoint == playerShipRight)
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}

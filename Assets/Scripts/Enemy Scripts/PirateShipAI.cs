using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShipAI : EnemyStats
{

    //VARIABLES
    private GameManager gm;
    private PlayerMovement player;

    private Vector3 _direction;
    private Transform playerShip;
    private Quaternion _lookRotation;
    public float rotateSpeed = 1.5f;
    public float moveSpeed = 20f;

    public float lookRadius = 180;
    private Transform alignPoint;

    public List<Transform> bulletEndsRight;
    public List<Transform> bulletEndsLeft;
    public Rigidbody bullet;
    public Transform barrelEnd;
    public float fireRate = 2f;
    float nextFire;

    public List<Transform> movePoints;
    public int currentPos = 0;

    public bool charging = false;
    public bool piercingBlowCheck = false;

    //public Transform frontCheck;
    //public Transform backCheck;
    //private Vector3 collisionPoint;
    //UPDATES

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        playerShip = FindObjectOfType<MovementControlsShip>().transform;
        player = FindObjectOfType<PlayerMovement>();
        enemyMaxHP = 400;
        enemyCurrentHP = 400;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, playerShip.position) < lookRadius && player.isControllingShip)
        {
            comeAlongSideShip();
            if (Vector3.Distance(transform.position, playerShip.position) < 120f && Vector3.Angle(_direction, transform.forward) <= 15)
            {
                piercingBlow();
                charging = true;
                piercingBlowCheck = true;
            } else if (Vector3.Distance(transform.position, playerShip.position) >= 120f && charging == true)
            {
                charging = false;
                piercingBlowCheck = false;
                moveSpeed = moveSpeed / 2.5f;
            }
        }
        else
        {
            Patrol();
        }

        if (enemyCurrentHP <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    //METHODS

    void comeAlongSideShip()
    {
         _direction = (playerShip.position - transform.position).normalized;
         _lookRotation = Quaternion.LookRotation(_direction);
         transform.position += transform.forward * moveSpeed * Time.deltaTime;

       if(!charging)
       {
          
         //Aiming towards player's ship
         transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, 8 * Time.deltaTime);

         if (Vector3.Angle(_direction, transform.forward) >= 45 && Vector3.Angle(_direction, transform.forward) <= 135)
         {
            if (transform.position.y <= playerShip.transform.position.y + 10 && transform.position.y >= playerShip.transform.position.y - 10)
            {
                ShootingCannons();
            }
         }

       }
    }

    private void piercingBlow()
    {
        if(!charging)
        {
        moveSpeed = 2.5f * moveSpeed;
        }
    }

    #region Cannons
    void ShootingCannons()
    {
        if (Time.time > nextFire)
        {

            nextFire = Time.time + fireRate;
            Rigidbody bulletInstance;
            foreach (Transform x in bulletEndsRight)
            {
                bulletInstance = Instantiate(bullet, x.position, x.rotation) as Rigidbody;
                bulletInstance.AddForce(barrelEnd.forward * 4500);
            }
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
        Gizmos.DrawWireSphere(transform.position, 120f);
    }

    private void Patrol()
    {
        if (Vector3.Distance(movePoints[currentPos].position, transform.position) < 5)
        {
            currentPos++;
            if (currentPos >= movePoints.Count)
            {
                currentPos = 0;
            }
        }
        _direction = (movePoints[currentPos].position - transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, 8 * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    
}

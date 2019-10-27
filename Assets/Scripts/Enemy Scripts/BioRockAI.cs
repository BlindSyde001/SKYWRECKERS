using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioRockAI : EnemyStats
{
    //VARIABLES
    private float lookRadius = 150f;
    private MovementControlsShip playerShip;
    public Transform swarmPoint;
    public Rigidbody swarmEnemy;

    private float cooldownTimer = 7f;
    public float cooldownTick = 0f;

    //UPDATES

    private void Awake()
    {
        enemyMaxHP = 120;
        enemyCurrentHP = 120;
        playerShip = FindObjectOfType<MovementControlsShip>();
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, playerShip.transform.position) < lookRadius)
        {
            SummonSwarm();
        }
        if(cooldownTick <= cooldownTimer)
           cooldownTick += Time.deltaTime;

        if (enemyCurrentHP <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    //METHODS
    public void SummonSwarm()
    {
        if(cooldownTick >= cooldownTimer)
        {

            Rigidbody swarmInstance;
            swarmInstance = Instantiate(swarmEnemy, swarmPoint.transform.position, swarmPoint.transform.rotation) as Rigidbody;
            swarmInstance.AddForce(swarmPoint.forward * 100);
            cooldownTick = 0;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}

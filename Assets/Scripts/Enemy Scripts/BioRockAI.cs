using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioRockAI : EnemyStats
{
    //VARIABLES
    private float lookRadius = 100f;
    private PlayerMovement playerShip;
    public GameObject swarmPoint;
    public GameObject swarmEnemy;

    private float cooldownTimer = 3f;
    private float cooldownTick;

    //UPDATES

    private void Awake()
    {
        playerShip = FindObjectOfType<PlayerMovement>();
    }
    private void Update()
    {
        if(Vector3.Distance(transform.position, playerShip.transform.position) < lookRadius)
        {
            SummonSwarm();
        }
    }

    //METHODS
    public void SummonSwarm()
    {
        if(cooldownTick >= cooldownTimer)
        {
            //shoot out swarm bullet thing.
        } else
        {
            cooldownTick += Time.deltaTime;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}

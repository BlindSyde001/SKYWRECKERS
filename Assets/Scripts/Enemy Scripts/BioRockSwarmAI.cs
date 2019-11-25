using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioRockSwarmAI : MonoBehaviour
{
    //VARIABLES
    private MovementControlsShip playerShip;
    private Rigidbody rb;
    //UPDATES

    private void Awake()
    {
        playerShip = FindObjectOfType<MovementControlsShip>();
        rb = transform.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        rb.velocity = Vector3.Normalize(playerShip.transform.position - transform.position) * 60f;
        
    }
    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ShipGround"))
        {
            other.GetComponent<ShipColliders>().damage = 5;
            other.GetComponent<ShipColliders>().dot = true;
            Destroy(this.gameObject);
        }
    }
}

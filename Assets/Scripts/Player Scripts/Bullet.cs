using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool player = false;
    public bool enemy = false;

    //UPDATES
    void Start()
    {
        Destroy(gameObject, 1f);
    }

    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy") && player)
        {
            if(other.GetComponentInParent<PirateShipAI>() != null)
            {
                other.GetComponentInParent<PirateShipAI>().enemyCurrentHP -= 5;
            }
            if(other.GetComponentInParent<BioRockAI>() != null)
            {
                other.GetComponentInParent<BioRockAI>().enemyCurrentHP -= 5;
            }
            Destroy(this.gameObject);
        }
        else if(other.gameObject.CompareTag("ShipGround") && enemy)
        {
            other.GetComponent<ShipColliders>().damage = 3;
            other.GetComponent<ShipColliders>().DamageTaken();
            print("CANNON SHOT");
            Destroy(this.gameObject);
        }
    }
}

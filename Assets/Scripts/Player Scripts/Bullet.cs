using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool player = false;
    public bool enemy = false;
    public GameObject smokeFX;

    //UPDATES
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy") && player)
        {
            Instantiate(smokeFX, transform.position, transform.rotation);
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
            Instantiate(smokeFX, transform.position, transform.rotation);
            other.GetComponent<ShipColliders>().damage = 3;
            other.GetComponent<ShipColliders>().DamageTaken();
            print("CANNON SHOT");
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //VARIABLES
    public bool player = false;
    public bool enemy = false;
    private AudioManager _audioManager;

    //UPDATES
    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
    }
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy") && player)
        {
            if (other.GetComponentInParent<PirateShipAI>() != null)
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
            _audioManager.PlaySFX(_audioManager.cannonHit);
            other.GetComponent<ShipColliders>().damage = 3;
            other.GetComponent<ShipColliders>().DamageTaken();
            print("CANNON SHOT");
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    //VARIABLES
    private GameManager gm;

    //UPDATES
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            print("CHECK POINT SAVED");
            gm.lastCheckpointPos = transform.position;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    //VARIABLES
    private GameManager gm;
    public GameObject dockPos;

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
            gm.lastCheckpointPosName = gameObject.name;
            gm.shipLastCheckpointPosName = dockPos.name;
        }
    }
}

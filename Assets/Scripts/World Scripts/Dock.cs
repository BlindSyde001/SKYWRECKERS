using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : MonoBehaviour
{
    //VARIABLES
    public Transform dockingPlacement;
    private MovementControlsShip ship;

    private Transform newShipPos;

    private void Start()
    {
        ship = FindObjectOfType<MovementControlsShip>();
    }

    private void Update()
    {
        if(ship.docking == true)
        {
            newShipPos.position = transform.position;
            newShipPos.rotation = transform.rotation;
        }
    }
}

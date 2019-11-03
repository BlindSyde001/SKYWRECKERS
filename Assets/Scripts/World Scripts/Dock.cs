using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : MonoBehaviour
{
    ////VARIABLES
    private UIManager ui;
    private PlayerMovement player;

    //UPDATES
    private void Awake()
    {
        ui = FindObjectOfType<UIManager>();
        player = FindObjectOfType<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ShipGround") && player.isControllingShip == true)
        {
            ui.UIObjectText.text = " Press G to Dock";
        } else
        {
            ui.UIObjectText.text = "";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ui.UIObjectText.text = "";
        }
    }
}

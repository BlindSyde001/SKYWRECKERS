using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : MonoBehaviour
{
    ////VARIABLES
    private UIManager ui;
    private PlayerMovement player;
    private bool mapReveal = false;
    public GameObject mapSection;

    //UPDATES
    private void Awake()
    {
        ui = FindObjectOfType<UIManager>();
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        if(ui.UIObjectText.text == "Press G to Dock" && (!player.isControllingShip))
        {
            ui.UIObjectText.text = "";
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ShipGround") && player.isControllingShip == true)
        {
            ui.UIObjectText.text = "Press G to Dock";

            if(!mapReveal)
            {
                mapSection.SetActive(true);
                mapReveal = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("ShipGround"))
        {
            ui.UIObjectText.text = "";
        }
    }
}

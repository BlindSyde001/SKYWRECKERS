﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourcePickUp : MonoBehaviour
{
    //VARIABLES
    private bool pickUpAllowed; //Bool to allow the player to pick up the gameobject

    public InventoryItem item; //Access to resource gameobjects in inventory script
    public string message; //Custom message that appears to pick up the resource gameobject


    public TextMeshProUGUI woodText;
    public TextMeshProUGUI fabricText;
    public TextMeshProUGUI metalText;

    //UPDATES
    void Start()
    {
        //Hasn't been set up yet
        TextMeshProUGUI woodText = GetComponent<TextMeshProUGUI>(); //getting access to the text
        TextMeshProUGUI fabricText = GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI metalText = GetComponent<TextMeshProUGUI>();
    }
    

    void FixedUpdate()
    {
        if (pickUpAllowed && Input.GetKeyDown(KeyCode.Q)) //Kept putting me back on the ship with E key
            PickUp();
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.name);
        if (collision.gameObject.name.Equals("Player")) //Text appears when Player is near item, through a collider
        {
            UIManager.Instance.ToggleText(message);
            pickUpAllowed = true; //Item can be picked up

        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name.Equals("Player")) //Text disappears when Player is not near the item, through a collider
        {
            UIManager.Instance.ToggleText("");
            pickUpAllowed = false;
        }
    }

    //METHODS
    private void PickUp()
    {

        if (item == InventoryItem.WOOD) //Item is picked up and appears in inventory
            Inventory.Instance.hasWood = true;
        if (item == InventoryItem.FABRIC)
            Inventory.Instance.hasFabric = true;
        if (item == InventoryItem.METAL)
            Inventory.Instance.hasMetal = true;
        UIManager.Instance.ToggleText("");
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryItem { WOOD, FABRIC, METAL }
public class Inventory : MonoBehaviour
{
    //VARIABLES
    public static Inventory Instance;

    public int woodCount;
    public int metalCount;
    public int fabricCount;

    //Bools to know whether the player has the resource objects
    public bool hasWood;
    public bool hasMetal;
    public bool hasFabric;

    //Pop up meesgae to get track how many of the resources you have
    //public string message;

    //Resorce Game Objects
    public GameObject Wood;
    public GameObject Fabric;
    public GameObject Metal;


    //UPDATES
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        hasWood = hasMetal = hasFabric = false; //Player has not picked up any items yet
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }


    public void CheckInventory() //Pre set up for when the game needs to check for an ship upgrade
    {
        if (woodCount == 10)
        {
            Debug.Log("Wood Upgrade Available");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryItem { WOOD, FABRIC, METAL }
public class Inventory : MonoBehaviour
{
    //VARIABLES
    public static Inventory Instance;

    public string message;

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
    public GameObject WoodUpgrade;
    public GameObject FabricUpgrade;
    public GameObject MetalUpgrade;


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
            //WoodUpgrade.gameObject.SetActive(true);
        }
    }

    //void OnTriggerStay(Collider other)
    //{
    //    UIManager.Instance.UpgradeToggleText(message);
    //    if (other.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.E)) //if player is in build area and has an item, they press Q and item is placed
    //    {
    //        CheckInventory();
    //        Debug.Log(name);
    //    }

    //}
}

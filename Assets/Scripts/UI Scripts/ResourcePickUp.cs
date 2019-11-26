using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourcePickUp : MonoBehaviour
{
    //VARIABLES
    private static GameManager gm;
    private bool pickUpAllowed; //Bool to allow the player to pick up the gameobject

    public InventoryItem item; //Access to resource gameobjects in inventory script
    public string message; //Custom message that appears to pick up the resource gameobject

    public TextMeshProUGUI woodText;
    public TextMeshProUGUI fabricText;
    public TextMeshProUGUI metalText;

    public bool startUpResourceCount = false;

    //UPDATES
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }
    void Start()
    {
        //Hasn't been set up yet
        //TextMeshProUGUI woodText = GetComponent<TextMeshProUGUI>(); //getting access to the text
        //TextMeshProUGUI fabricText = GetComponent<TextMeshProUGUI>();
        //TextMeshProUGUI metalText = GetComponent<TextMeshProUGUI>();
    }
    

    void FixedUpdate()
    {
        if (pickUpAllowed && Input.GetKeyDown(KeyCode.E)) //Kept putting me back on the ship with E key
            PickUp();
    }

    private void LateUpdate()
    {
        if(startUpResourceCount == false)
        {
         Debug.Log("Count Instance");
         woodText.text = ("Wood " + Inventory.Instance.woodCount + "/10");
         fabricText.text = ("Fabric " + Inventory.Instance.fabricCount + "/10");
         metalText.text = ("Metal " + Inventory.Instance.metalCount + "/10");
         startUpResourceCount = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log(collision.name);
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
        //Inventory.Instance.hasLock = true;
        if (item == InventoryItem.WOOD) //Item is picked up and appears in inventory 
        {
            gm.currentWoodCount++;
            Inventory.Instance.woodCount++;
            Inventory.Instance.CheckInventory();
            woodText.text = ("Wood " + Inventory.Instance.woodCount + "/10");
            if (Inventory.Instance.woodCount == 10)
            {
                print("Getting rid of text");
                //Inventory.Instance.upgradeAvailableText.gameObject.SetActive(true);
                Inventory.Instance.StartCoroutine(Inventory.Instance.UpgradeAvailableTextGone());
            }
        } 
        
        if (item == InventoryItem.FABRIC)
        {
            gm.currentFabricCount++;
            Inventory.Instance.fabricCount++;
            Inventory.Instance.CheckInventory();
            fabricText.text = ("Fabric " + Inventory.Instance.fabricCount + "/10");
        }
        if (item == InventoryItem.METAL)
        {
            gm.currentMetalCount++;
            Inventory.Instance.metalCount++;
            Inventory.Instance.CheckInventory();
            metalText.text = ("Metal " + Inventory.Instance.metalCount + "/10");
        }

        UIManager.Instance.ToggleText("");
        gameObject.SetActive(false);
        gm.BoolUpdate();
    }
}

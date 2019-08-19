using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    //upgrade Buttons + Panel
    public GameObject woodButton;
    public GameObject fabricButton;
    public GameObject metalButton;
    public GameObject upgradePanel;

    //Trigger text
    public TextMeshProUGUI upgradeTableText;

    //Place Holder Upgrades for Testing
    public GameObject shipUpgradeWood;
    public GameObject shipUpgradeFabric;
    public GameObject shipUpgradeMetal;

    //UPDATES
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //upgradeTableText.text = ""; //Text is blank to start the game
        hasWood = hasMetal = hasFabric = false; //Player has not picked up any items yet
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void CursorOn()
    {
        Cursor.visible = true; //Turing cursor on so players can interact with menu
    }


    public void ShipUpgradeSettingsOff() //Turn off all settings for the upgrade panel
    {
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        upgradePanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            //upgradeTableText.text = "Press [E] To Access Ship Upgrades";
            upgradeTableText.gameObject.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        upgradeTableText.gameObject.SetActive(true);
        if (other.gameObject.name.Equals("Player") && Input.GetKeyDown(KeyCode.E))
        {
            //upgradeTableText.text = "Press [E] To Access Ship Upgrades";
            upgradePanel.SetActive(true);
            CursorOn();
            Time.timeScale = 0f;
            CheckInventory();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            //upgradeTableText.text = "";
            upgradeTableText.gameObject.SetActive(true);
            upgradePanel.SetActive(false);
        }
    }

    public void CheckInventory() //Pre set up for when the game needs to check for an ship upgrade
    {
        if (woodCount == 10)
        {
            //Debug.Log("Wood Upgrade Available");
            woodButton.SetActive(true);
        }

        if (fabricCount == 10)
        {
            //Debug.Log("Fabric Upgrade Available");
            fabricButton.SetActive(true);
        }

        if (metalCount == 10)
        {
            //Debug.Log("Metal Upgrade Available");
            metalButton.SetActive(true);
        }
    }

    public void WoodUpgrade()
    {
        shipUpgradeWood.SetActive(true); //Place holder to see if the function works
    }

    public void FabricUpgrade()
    {
        //Debug.Log("Fabric Upgrade Complete");
        shipUpgradeFabric.SetActive(true);
    }

    public void MetalUpgrade()
    {
        //Debug.Log("Metal Upgrade Complete");
        shipUpgradeMetal.SetActive(true);
    }
}

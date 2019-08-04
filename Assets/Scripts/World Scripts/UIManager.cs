using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI UIObjectText; //Text that appears when player is near resource gameobject

    public TextMeshProUGUI UIUpgradeText; //Text that appears when player is near upgrade table

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIObjectText.text = ""; //Text is blank at the start of the game

        UIUpgradeText.text = "";
    }

    public void ToggleText(string itemName) //Toggle text is individually set in inspector for each item
    {
        UIObjectText.text = itemName;
    }

    public void UpgradeToggleText(string itemName) //Toggle text is individually set in inspector for each item
    {
        UIUpgradeText.text = itemName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

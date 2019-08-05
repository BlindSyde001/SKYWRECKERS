using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //VARIABLES
    public static UIManager Instance;
    public TextMeshProUGUI UIObjectText; //Text that appears when player is near resource gameobject

    #region SHIP HP
    public TextMeshProUGUI leftCannonHPText;
    public TextMeshProUGUI righCannonHPText;
    public TextMeshProUGUI sailsHPText;
    public TextMeshProUGUI hullHPText;

    public float leftCannonHP = 100f;
    public float rightCannonHP = 100f;
    public float sailsHP = 100f;
    public float hullHP = 100f;
    #endregion

    //UPDATES
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UIObjectText.text = ""; //Text is blank at the start of the game
    }
    void Update()
    {
        leftCannonHPText.text = leftCannonHP + "/100";
        righCannonHPText.text = rightCannonHP + "/100";
        sailsHPText.text = sailsHP + "/100";
        hullHPText.text = hullHP + "/100";

        if(leftCannonHP < 5 || rightCannonHP < 5 || sailsHP < 5 || hullHP < 5)
        {
            Time.timeScale = 0;
            Debug.Log("YOU LOSE!");
        }
    }


    //METHODS
    public void ToggleText(string itemName) //Toggle text is individually set in inspector for each item
    {
        UIObjectText.text = itemName;
    }
}

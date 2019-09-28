using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //VARIABLES
    public static UIManager Instance;
    public TextMeshProUGUI UIObjectText; //Text that appears when player is near resource gameobject
    public GameObject gameover;

    #region SHIP HP
    public TextMeshProUGUI leftCannonHPText;
    public TextMeshProUGUI righCannonHPText;
    public TextMeshProUGUI sailsHPText;
    public TextMeshProUGUI hullHPText;

    public int leftCannonHP = 100;
    public int leftCannonMaxHP = 100;
    public int rightCannonHP = 100;
    public int rightCannonMaxHP = 100;
    public int sailsHP = 100;
    public int sailsMaxHP = 100;
    public int hullHP = 100;
    public int hullMaxHP = 100;
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
        leftCannonHPText.text = "LC " + leftCannonHP + "/" + leftCannonMaxHP;
        righCannonHPText.text = "RC " + rightCannonHP + "/" + rightCannonHP;
        sailsHPText.text = "SL " + sailsHP + "/" + sailsMaxHP;
        hullHPText.text = "HL " + hullHP + "/" + hullMaxHP;

        if(leftCannonHP < 5 || rightCannonHP < 5 || sailsHP < 5 || hullHP < 5)
        {
            Time.timeScale = 0;
            gameover.SetActive(true);
            Debug.Log("YOU LOSE!");
        }
        
        leftCannonHP = Mathf.Clamp(leftCannonHP, 0, leftCannonMaxHP);
        rightCannonHP = Mathf.Clamp(rightCannonHP, 0, rightCannonMaxHP);
        hullHP = Mathf.Clamp(hullHP, 0, hullMaxHP);
        sailsHP = Mathf.Clamp(sailsHP, 0, sailsMaxHP);
    }


    //METHODS
    public void ToggleText(string itemName) //Toggle text is individually set in inspector for each item
    {
        UIObjectText.text = itemName;
    }
}

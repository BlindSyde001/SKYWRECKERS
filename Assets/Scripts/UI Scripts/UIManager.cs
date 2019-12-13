using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //VARIABLES
    public static UIManager Instance;
    public TextMeshProUGUI UIObjectText; //Text that appears when player is near resource gameobject
    public GameObject gameover;

    public int objectiveMarkers = 0;
    public GameObject endGameCloudWall;
    public List<ParticleSystem> endGameClouds;

    #region UI Image
    public Image shipControls;
    public Image playerControls;
    public Image climbingControls;
    #endregion
    #region UI HP Image
    public List<GameObject> green;
    public List<GameObject> yellow;
    public List<GameObject> red;
    #endregion
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
        shipControls.canvasRenderer.SetAlpha(0);
        playerControls.canvasRenderer.SetAlpha(0);
        climbingControls.canvasRenderer.SetAlpha(0);
    }

    void Start()
    {
        UIObjectText.text = ""; //Text is blank at the start of the game
    }
    void Update()
    {
        leftCannonHPText.text = "LC " + leftCannonHP + "/" + leftCannonMaxHP;
        righCannonHPText.text = "RC " + rightCannonHP + "/" + rightCannonMaxHP;
        sailsHPText.text = "SL " + sailsHP + "/" + sailsMaxHP;
        hullHPText.text = "HL " + hullHP + "/" + hullMaxHP;

        leftCannonHP = Mathf.Clamp(leftCannonHP, 0, leftCannonMaxHP);
        rightCannonHP = Mathf.Clamp(rightCannonHP, 0, rightCannonMaxHP);
        hullHP = Mathf.Clamp(hullHP, 0, hullMaxHP);
        sailsHP = Mathf.Clamp(sailsHP, 0, sailsMaxHP);
    }

    private void FixedUpdate()
    {

        if (leftCannonHP <= 0 || rightCannonHP <= 0 || sailsHP <= 0 || hullHP <= 0)
        {
            Time.timeScale = 0;
            gameover.SetActive(true);
            Cursor.visible = true;
        }
        #region HP Ui Change
        #region LC
        if(leftCannonHP <= 50 && leftCannonHP > 25)
        {
            yellow[0].SetActive(true);
            red[0].SetActive(false);
            if(leftCannonHP <= 25)
            {
                red[0].SetActive(true);
            }else if(leftCannonHP > 50)
            {
                green[0].SetActive(true);
                yellow[0].SetActive(false);
                red[0].SetActive(false);
            }
        }
        #endregion
        #region RC
        if (rightCannonHP <= 50 && rightCannonHP > 25)
        {
            yellow[1].SetActive(true);
            red[1].SetActive(false);
            if (rightCannonHP <= 25)
            {
                red[1].SetActive(true);
            }
            else if (rightCannonHP > 50)
            {
                green[1].SetActive(true);
                yellow[1].SetActive(false);
                red[1].SetActive(false);
            }
        }
        #endregion
        #region Hull
        if (hullHP <= 50 && hullHP > 25)
        {
            yellow[2].SetActive(true);
            red[2].SetActive(false);
            if (hullHP <= 25)
            {
                red[2].SetActive(true);
            }
            else if (hullHP > 50)
            {
                green[2].SetActive(true);
                yellow[2].SetActive(false);
                red[2].SetActive(false);
            }
        }
        #endregion
        #region Sails
        if (sailsHP <= 50 && sailsHP > 25)
        {
            yellow[3].SetActive(true);
            red[3].SetActive(false);
            if (sailsHP <= 25)
            {
                red[3].SetActive(true);
            }
            else if (sailsHP > 50)
            {
                green[3].SetActive(true);
                yellow[3].SetActive(false);
                red[3].SetActive(false);
            }
        }
        #endregion
        #endregion
    }

    private void LateUpdate()
    {
        if (Inventory.Instance.woodCount >= 5 && Inventory.Instance.metalCount >= 5 && Inventory.Instance.fabricCount >= 5)
        {
            endGameCloudWall.GetComponent<EndGameWall>().enabled = true;
            endGameCloudWall.GetComponent<BoxCollider>().isTrigger = true;
            foreach(ParticleSystem a in endGameClouds)
            {
                a.enableEmission = false;
            }
        }
    }
    //METHODS
    public void ToggleText(string itemName) //Toggle text is individually set in inspector for each item
    {
        UIObjectText.text = itemName;
    }


    public IEnumerator UiImage(Image x)
    {
        x.canvasRenderer.SetAlpha(1);
        yield return new WaitForSeconds(1);
        x.CrossFadeAlpha(0, 3, false);
        yield return null;
    }
}

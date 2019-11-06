using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Repair : MonoBehaviour
{
    //VARIABLES
    public bool on = false;
    public bool sails = false;
    public bool hull = false;
    public bool left = false;
    public bool right = false;

    public float f;

    private UIManager UI;
    public Slider healthSlider;
    public GameObject ToolTip;

    //UPDATES
    private void Awake()
    {
        UI = FindObjectOfType<UIManager>();
    }
    private void Update()
    {
        RepairAreas();
        RepairIconActivate();
    }

    //METHODS

    void RepairAreas()
    {
        #region Repair Parts
        if (on)
        {
            print("ON");
            f += 3 * Time.deltaTime;
            healthSlider.gameObject.SetActive(true);
            if (sails)
            {
                print("sails");
                healthSlider.maxValue = UI.GetComponent<UIManager>().sailsMaxHP;
                healthSlider.value = UI.GetComponent<UIManager>().sailsHP;
                if (f >= 1)
                {
                    UI.GetComponent<UIManager>().sailsHP += Mathf.FloorToInt(f);
                    f = 0;
                }
            }
            if (hull)
            {
                print("hull");
                healthSlider.maxValue = UI.GetComponent<UIManager>().hullMaxHP;
                healthSlider.value = UI.GetComponent<UIManager>().hullHP;
                if (f >= 1)
                {
                    UI.GetComponent<UIManager>().hullHP += Mathf.FloorToInt(f);
                    f = 0;
                }
            }
            if (left)
            {
                print("left");
                healthSlider.maxValue = UI.GetComponent<UIManager>().leftCannonMaxHP;
                healthSlider.value = UI.GetComponent<UIManager>().leftCannonHP;
                if (f >= 1)
                {
                    UI.GetComponent<UIManager>().leftCannonHP += Mathf.FloorToInt(f);
                    f = 0;
                }
            }
            if (right)
            {
                print("right");
                healthSlider.maxValue = UI.GetComponent<UIManager>().rightCannonMaxHP;
                healthSlider.value = UI.GetComponent<UIManager>().rightCannonHP;
                if (f >= 1)
                {
                    UI.GetComponent<UIManager>().rightCannonHP += Mathf.FloorToInt(f);
                    f = 0;
                }
            }
        } //else if(!on)
        //{
        //    print("OFF");
        //    healthSlider.gameObject.SetActive(false);
        //}
        #endregion
    }

    void RepairIconActivate()
    {
        if (sails && UI.GetComponent<UIManager>().sailsHP < UI.GetComponent<UIManager>().sailsMaxHP 
            || hull && UI.GetComponent<UIManager>().hullHP < UI.GetComponent<UIManager>().hullMaxHP 
            || left && UI.GetComponent<UIManager>().leftCannonHP < UI.GetComponent<UIManager>().leftCannonMaxHP
            || right && UI.GetComponent<UIManager>().rightCannonHP < UI.GetComponent<UIManager>().rightCannonMaxHP)
        {
            ToolTip.SetActive(true);
        } else
        {
            ToolTip.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            UI.UIObjectText.text = "Repair Zone";
        }
    }
}

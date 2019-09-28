using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //UPDATES
    private void Awake()
    {
        UI = FindObjectOfType<UIManager>();
    }
    private void Update()
    {
        if(on == true)
        {
            f += 3 * Time.deltaTime;
            if (sails)
            {
                if(f >= 1)
                {
                    UI.GetComponent<UIManager>().sailsHP += Mathf.FloorToInt(f);
                    f = 0;
                }
            }
            if (hull)
            {
                if (f >= 1)
                {
                    UI.GetComponent<UIManager>().hullHP += Mathf.FloorToInt(f);
                    f = 0;
                }
            }
            if (left)
            {
                if (f >= 1)
                {
                    UI.GetComponent<UIManager>().leftCannonHP += Mathf.FloorToInt(f);
                    f = 0;
                }
            }
            if (right)
            {
                if (f >= 1)
                {
                    UI.GetComponent<UIManager>().rightCannonHP += Mathf.FloorToInt(f);
                    f = 0;
                }
            }
        }
    }

    //METHODS
}

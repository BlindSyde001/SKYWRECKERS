using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipColliders : MonoBehaviour
{
    //VARIABLES
    private UIManager UI;
    public UIManager hitPos;

    public bool left = false;
    public bool right = false;
    public bool hull = false;
    public bool sail = false;
    MovementControlsShip ship;
    //UPDATES
    private void Awake()
    {
        UI = FindObjectOfType<UIManager>();
        ship = FindObjectOfType<MovementControlsShip>();
    }

    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if (ship.docking == false)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Ground"))
            {
                if (sail)
                {
                    UI.GetComponent<UIManager>().sailsHP -= 5;
                    Debug.Log("HIT SAIL");
                }


                if (hull)
                {
                    UI.GetComponent<UIManager>().hullHP -= 5;
                    Debug.Log("HIT HULL");
                }


                if (left)
                {
                    UI.GetComponent<UIManager>().leftCannonHP -= 5;
                    Debug.Log("HIT LEFT");
                }


                if (right)
                {
                    UI.GetComponent<UIManager>().rightCannonHP -= 5;
                    Debug.Log("HIT RIGHT");
                }
            }
        }
    }
}

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

    public int damage;
    public bool dot = false;

    //UPDATES
    private void Awake()
    {
        UI = FindObjectOfType<UIManager>();
        ship = FindObjectOfType<MovementControlsShip>();
    }

    private void Update()
    {
        if (dot)
        {
            DOTDamage();
        }
    }

    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if (ship.docking == false)
        {
            #region Colliding
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
            #endregion
        }
    }

    public void DamageTaken()
    {
        if (sail)
        {
            UI.GetComponent<UIManager>().sailsHP -= damage;
            Debug.Log("HIT SAIL");
        }


        if (hull)
        {
            UI.GetComponent<UIManager>().hullHP -= damage;
            Debug.Log("HIT HULL");
        }


        if (left)
        {
            UI.GetComponent<UIManager>().leftCannonHP -= damage;
            Debug.Log("HIT LEFT");
        }


        if (right)
        {
            UI.GetComponent<UIManager>().rightCannonHP -= damage;
            Debug.Log("HIT RIGHT");
        }
    }
    public void DOTDamage()
    {
        float timer = 3f;
        float timerTick = 0f;
        int dotTicks = 0;

        timerTick += Time.deltaTime;
        if(dotTicks >= 3)
        {
            dot = false;
        }
        if(timerTick >= timer)
        {
            DamageTaken();
            dotTicks++;
            timerTick = 0;
        }
    }
}

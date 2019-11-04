using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipColliders : MonoBehaviour
{
    //VARIABLES
    private UIManager UI;
    public UIManager hitPos;
    MovementControlsShip ship;

    public bool left = false;
    public bool right = false;
    public bool hull = false;
    public bool sail = false;
    

    public int damage;
    public bool dot = false;

    float timer = 3f;
    float timerTick = 0f;
    int dotTicks = 0;

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
            float timerTickr = 0;
            timerTickr += Time.deltaTime;

            if(timerTickr >= 1)
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
                    timerTickr = 0;
                }
                #endregion
            }
           
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

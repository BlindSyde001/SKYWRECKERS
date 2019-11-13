using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandCollision : MonoBehaviour
{
    MovementControlsShip ship;
    UIManager UI;

    private void Awake()
    {
        ship = FindObjectOfType<MovementControlsShip>();
        UI = FindObjectOfType<UIManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            print("SOMETHING HAPPENED");
            Vector3 direction = (transform.position - other.transform.position).normalized;
            direction.y = 0;
            ship.nudgeVector = direction * 50f;
            UI.sailsHP -= 5;
            UI.rightCannonHP -= 5;
            UI.leftCannonHP -= 5;
            UI.hullHP -= 5;
            
        }
    }
}

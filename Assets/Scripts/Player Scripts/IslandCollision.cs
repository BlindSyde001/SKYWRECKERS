using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandCollision : MonoBehaviour
{
    MovementControlsShip ship;

    private void Awake()
    {
        ship = FindObjectOfType<MovementControlsShip>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            print("SOMETHING HAPPENED");
            Vector3 direction = (transform.position - other.transform.position).normalized;
            direction.y = 0;
            ship.nudgeVector = direction * 50f;
        }
    }
}

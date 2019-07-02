using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    //VARIABLES
    public GameObject player;

    public Rigidbody rb;


    //UPDATES
    void Start()
    {
         rb = player.GetComponent<Rigidbody>();
    }

    //METHODS
    public void OnTriggerStay(Collider other)
    {
        
        if(Input.GetKey(KeyCode.L))
        {
            
            rb.useGravity = false;
            player.transform.Translate((Vector3.up *5) * Time.deltaTime, Space.World);
            Debug.Log("Climbing!");
        }
        else
        {
            rb.useGravity = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        rb.useGravity = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    public GameObject playerCharacter;

    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
         rb = playerCharacter.GetComponent<Rigidbody>();
    }

    public void OnTriggerStay(Collider other)
    {
        
        if(Input.GetKey(KeyCode.L))
        {
            
            rb.useGravity = false;
            playerCharacter.transform.Translate((Vector3.up *5) * Time.deltaTime, Space.World);
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

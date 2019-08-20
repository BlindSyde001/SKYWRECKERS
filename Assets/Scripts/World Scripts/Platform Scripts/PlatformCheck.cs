using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCheck : MonoBehaviour
{
    public GameObject Player;
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Platform")
        {
            //This will make the player a child of the Obstacle
            Player.transform.parent = other.gameObject.transform; //Change "myPlayer" to your player
            //Debug.Log("Stuck on Platform!");
         
        }
    }
    //Note : Remember to remove the player from the Obstacle's child list when you jump or leave it

    void OnTriggerExit(Collider other)
    {
        Player.transform.parent = null;
        //Debug.Log("Left the platform!");
    }
}

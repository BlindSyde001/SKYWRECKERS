using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // public GameObject Player;
    //public Transform[] Waypoints;

    //public float platformSpeed = 2;
    //Vector3 platformPosition;
    //    public int CurrentPoint = 0;
    ////public float posY;
    ////public float posZ;
    ////https://answers.unity.com/questions/827225/moving-platform-help-c.html

    //private void Start()
    //{

    //}

    //void FixedUpdate()
    //{
    //   // new Vector3 platformPosition(0,transform.position.y, transform.position.z); //error when I opened game
    //    if(transform.position.z != Waypoints[CurrentPoint].transform.position.z)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, Waypoints[CurrentPoint].transform.position, platformSpeed * Time.deltaTime);
    //    }

    //    if(transform.position.z == Waypoints[CurrentPoint].transform.position.z)
    //    {
    //        CurrentPoint += 1;
    //    }
    //    if(CurrentPoint >= Waypoints.Length)
    //    {
    //        CurrentPoint = 0;
    //    }


    //}

    public Transform[] Waypoints;
    public float moveSpeed = 3;
    //public float rotateSpeed = 0.5f;
    //public float scaleSpeed = 0f;
    public int CurrentPoint = 0;

    private void Awake()
    {
    }

    void FixedUpdate()
    {

        if (transform.position != Waypoints[CurrentPoint].transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[CurrentPoint].transform.position, moveSpeed * Time.deltaTime);
           // transform.rotation = Quaternion.Lerp(transform.rotation, Waypoints[CurrentPoint].transform.rotation, rotateSpeed * Time.deltaTime);
            //transform.localScale = Vector3.Lerp(transform.localScale, Waypoints[CurrentPoint].transform.localScale, scaleSpeed * Time.deltaTime);
        }

        if (transform.position == Waypoints[CurrentPoint].transform.position)
        {
            CurrentPoint += 1;
        }
        if (CurrentPoint >= Waypoints.Length)
        {
            CurrentPoint = 0;
        }
    }
//    void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.tag == "Platform")
//        {
//            //This will make the player a child of the Obstacle
//            Player.transform.parent = other.gameObject.transform; //Change "myPlayer" to your player
//        }
//    }
//    //Note : Remember to remove the player from the Obstacle's child list when you jump or leave it

//void OnTriggerExit(Collider other)
//    {
//        Player.transform.parent = null;
//    }
}

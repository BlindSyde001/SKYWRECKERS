using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceUI : MonoBehaviour
{
    //VARIABLES
    private PlayerMovement player;

    //UPDATES
    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

        //Vector3 direction = (player.transform.position - transform.position);
        //Ray ray = new Ray(transform.position, direction);
        //Debug.DrawRay(transform.position, direction);

        //transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
    }
    //METHODS
}

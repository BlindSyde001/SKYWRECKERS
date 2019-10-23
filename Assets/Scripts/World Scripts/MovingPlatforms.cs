using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    //UPDATES
    private void Update()
    {
        transform.position = transform.position + transform.right * Mathf.Sin(Time.time * 2) * 1/10;
    }
}

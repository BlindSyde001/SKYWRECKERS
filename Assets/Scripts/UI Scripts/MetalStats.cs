using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalStats : MonoBehaviour
{
    public GameObject metalStatistics;


    public void OnOver()
    {
        metalStatistics.SetActive(true);
    }

    public void OnExit()
    {
        metalStatistics.SetActive(false);
    }
}

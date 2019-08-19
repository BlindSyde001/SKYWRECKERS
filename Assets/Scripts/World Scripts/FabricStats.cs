using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FabricStats : MonoBehaviour
{
    public GameObject fabricStatistics;


    public void OnOver()
    {
        fabricStatistics.SetActive(true);
    }

    public void OnExit()
    {
        fabricStatistics.SetActive(false);
    }
}

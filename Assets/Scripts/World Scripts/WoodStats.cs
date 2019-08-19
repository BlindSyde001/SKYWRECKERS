using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodStats : MonoBehaviour
{
    public GameObject woodStatistics;


    public void OnOver()
    {
        woodStatistics.SetActive(true);
    }

    public void OnExit()
    {
        woodStatistics.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reload : MonoBehaviour
{
    //VARIABLES
    private GameManager gm;

    //UPDATES
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    //METHODS
    private void OnLevelWasLoaded(int level)
    {
        if(level == 1)
        {
            gm.UpdateList();
        }
    }
}

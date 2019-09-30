using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //VARIABLES
    private static GameManager instance;
    private static GameObject managers;
    public GameObject lastCheckpointPos;
    public GameObject shipLastCheckpointPos;

    //UPDATES
    private void Awake()
    {
        
        if (instance == null)
        {
            print("START POSITIONS");
            instance = this;
            DontDestroyOnLoad(managers);
            lastCheckpointPos = GameObject.Find("Start Point");
            shipLastCheckpointPos = GameObject.Find("Start DockPlacement");
        }
        else {
            Destroy(gameObject); }

    }
    void Start()
    {
        Cursor.visible = false;
    }   
}

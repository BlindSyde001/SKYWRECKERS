using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //VARIABLES
    private static GameManager instance;
    public Vector3 lastCheckpointPos;
    public Vector3 shipLastCheckpointPos;
    public List<GameObject> enemyList; //currently redundant but put can be used later
    public List<GameObject> resourceMasterList;
    public List<GameObject> resourceCurrentList;

    //UPDATES
    private void Awake()
    {
        if (instance == null)
        {
            print("START POSITIONS");
            instance = this;
            DontDestroyOnLoad(instance);
            lastCheckpointPos = GameObject.Find("Start Point").transform.position;
            shipLastCheckpointPos = GameObject.Find("Start DockPlacement").transform.position;

            foreach (GameObject enemies in GameObject.FindGameObjectsWithTag("PIRATE"))
                enemyList.Add(enemies);
            foreach (GameObject _resource in GameObject.FindGameObjectsWithTag("RESOURCE"))
                resourceCurrentList.Add(_resource);
        }
        else {
            Destroy(gameObject); }
    }
    void Start()
    {
        Cursor.visible = false;
            foreach (GameObject _resource in GameObject.FindGameObjectsWithTag("RESOURCE"))
            {
                resourceMasterList.Add(_resource);
            }
    }
}

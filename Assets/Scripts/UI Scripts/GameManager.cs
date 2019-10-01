using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //VARIABLES
    private static GameManager instance;
    public Vector3 lastCheckpointPos;
    public Vector3 shipLastCheckpointPos;
    public List<GameObject> resourceMasterList;
    public List<bool> resourceTaken;

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
        }
        else {
            Destroy(gameObject); }
    }
    void Start()
    {
        Cursor.visible = false;
        UpdateList();
        foreach(GameObject _resource in resourceMasterList)
        {
            resourceTaken.Add(_resource.activeSelf);
        }
    }

    //METHODS
    public void UpdateList()
    {
        foreach (GameObject _resource in GameObject.FindGameObjectsWithTag("RESOURCE"))
            resourceMasterList.Add(_resource);
    }
}

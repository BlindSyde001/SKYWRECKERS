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

    public int savedWoodCount;
    public int savedMetalCount;
    public int savedFabricCount;

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
        print("START");
        Cursor.visible = false;
        if(resourceMasterList.Count == 0)
        {
             foreach (GameObject _resource in GameObject.FindGameObjectsWithTag("RESOURCE"))
             { resourceMasterList.Add(_resource); }
        }
        if(resourceTaken.Count == 0)
        {
             foreach (GameObject _resource in resourceMasterList)
             { resourceTaken.Add(_resource.activeSelf); }
        }
    }

    //METHODS
    public void UpdateList()
    {
        print("updated");
        resourceMasterList.Clear();
        foreach (GameObject _resource in GameObject.FindGameObjectsWithTag("RESOURCE"))
        { resourceMasterList.Add(_resource); }
        
            for (int i = 0; i < resourceMasterList.Count; i++)
            {
                if (resourceMasterList[i].activeSelf != resourceTaken[i])
                {
                    resourceMasterList[i].SetActive(resourceTaken[i]);
                }
            }
    }

    public void BoolUpdate()
    {
        for (int i = 0; i < resourceTaken.Count; i++)
        {
            if (resourceMasterList[i].activeSelf != resourceTaken[i])
            {
                resourceTaken[i] = resourceMasterList[i].activeSelf;
            }
        }
    }
}

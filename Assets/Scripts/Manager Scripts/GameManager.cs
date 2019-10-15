using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    //VARIABLES
    private static GameManager instance;
    public GameObject lastCheckpointPos;
    public GameObject shipLastCheckpointPos;
    public List<GameObject> resourceMasterList;
    public List<bool> resourceTaken;

    public int savedWoodCount;
    public int savedMetalCount;
    public int savedFabricCount;

    public SaveState S1;
    public SaveState S2;
    public SaveState S3;

    public string lastCheckpointPosName;
    public string shipLastCheckpointPosName;

    public bool newGame = false;

    //UPDATES
    private void Awake()
    {
        if (instance == null && instance != this)
        {
            print("START POSITIONS");
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else { Destroy(gameObject); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            print("Load");
            LoadGameFile();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("Saved");
            SaveGameFile();
        }
    }

    //METHODS
    #region Playthrough Saves
    public void UpdateList()
    {
        print("updated");
        resourceMasterList.Clear();
        resourceMasterList.AddRange(GameObject.FindGameObjectsWithTag("RESOURCE"));

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
                resourceTaken[i] = resourceMasterList[i].activeSelf;
        }
    }
    #endregion

    #region Save & Load

    public void SaveGameFile()
    {
        S1.checkPointSaved = lastCheckpointPosName;
        S1.shipCheckPointSaved = shipLastCheckpointPosName;
        S1.resourceTakenSaved = resourceTaken;

        FileStream fs = new FileStream("Save.dat", FileMode.Create); //make save file
        BinaryFormatter bf = new BinaryFormatter(); // save file ino binary
        bf.Serialize(fs, S1); // just add this line afterwards
        fs.Close();
    }

    public void LoadGameFile()
    {
        using (Stream stream = File.Open("Save.dat", FileMode.Open)) //Opens file of name Save 1
        {
            var bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter(); // Just add

            S1 = (SaveState)bFormatter.Deserialize(stream); //Overrides the current version of "S1" with the saved version.
        }

        lastCheckpointPosName = S1.checkPointSaved;
        shipLastCheckpointPosName = S1.shipCheckPointSaved;
        resourceTaken = S1.resourceTakenSaved;
    }
    #endregion

    private void OnLevelWasLoaded(int level)
    {
        
        if (level == 1)
        {
            if(newGame)
            {
                lastCheckpointPos = GameObject.Find("Start Point");
                shipLastCheckpointPos = GameObject.Find("Start DockPlacement");
                lastCheckpointPosName = lastCheckpointPos.name;
                shipLastCheckpointPosName = shipLastCheckpointPos.name;
                resourceMasterList.Clear();
                if (resourceMasterList.Count == 0)
                {
                    resourceMasterList.AddRange(GameObject.FindGameObjectsWithTag("RESOURCE"));
                }
                resourceTaken.Clear();
                if (resourceTaken.Count == 0)
                {
                    foreach (GameObject _resource in resourceMasterList)
                    { resourceTaken.Add(_resource.activeSelf); }
                }
            }
            else
            {
                lastCheckpointPos = GameObject.Find(lastCheckpointPosName);
                shipLastCheckpointPos = GameObject.Find(shipLastCheckpointPosName);
                UpdateList();
            }
        }
    }
}

[System.Serializable]
public class SaveState
{
    [SerializeField]
    public List<bool> resourceTakenSaved;
    [SerializeField]
    public string checkPointSaved;
    [SerializeField]
    public string shipCheckPointSaved;
}

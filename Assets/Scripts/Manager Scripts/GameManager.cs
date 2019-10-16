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

    public List<SaveState> sFile;

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

    public void SaveGameFile(int x)
    {
        sFile[x].checkPointSaved = lastCheckpointPosName;
        sFile[x].shipCheckPointSaved = shipLastCheckpointPosName;
        sFile[x].resourceTakenSaved = resourceTaken;

        FileStream fs = new FileStream("Save File " + x + ".dat", FileMode.Create); //make save file
        BinaryFormatter bf = new BinaryFormatter(); // save file ino binary
        bf.Serialize(fs, sFile[x]); // just add this line afterwards
        fs.Close();
    }

    public void LoadGameFile(int x)
    {
        using (Stream stream = File.Open("Save File " + x + ".dat", FileMode.Open)) //Opens file of name Save 1
        {
            var bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter(); // Just add

            sFile[x] = (SaveState)bFormatter.Deserialize(stream); //Overrides the current version of "S1" with the saved version.
        }

        lastCheckpointPosName = sFile[x].checkPointSaved;
        shipLastCheckpointPosName = sFile[x].shipCheckPointSaved;
        resourceTaken = sFile[x].resourceTakenSaved;
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

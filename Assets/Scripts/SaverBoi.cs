using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Systems you need...
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System.Linq;


public class SaverBoi : MonoBehaviour
{
    public List<GameObject> AA;
    public List<bool> BB;
    public GameObject player;
    public GameObject checkPoint;
    public SaveState S1;

    private bool Loading = false;

    void Awake() // On awake, we're going to compile the default state of the level. We will compare our 'SaveState' with this.
    {

        // You can manually add resources to a list if you want fine control, otherwise we should just automate it.
        AA.AddRange(GameObject.FindGameObjectsWithTag("Test"));

        /* Below I am sorting the new list by distance from this 'Saver' GameObject.
        This will ensure the List is always consistent/in the same order.
        Saves will not be consistent if you adjust the position of a resource in the Editor and then load the game.
        This however, is not be an issue in a packaged game. */
        AA = AA.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToList();
        // Creating the generic 'active' state list for all objects in AA.
        BB = new List<bool>(new bool[AA.Count]);
        int i = 0;
        foreach (GameObject a in AA){
            BB[i] = true;
            i++;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("SAVING GAME");
            SAVEGAME();
        }
        if (Input.GetKeyDown(KeyCode.C) && !Loading)
        {
            print("LOADING GAME");
            Loading = true;
            LOADGAME();
        }
    }
    void SAVEGAME()
    {
        int i = 0;
        foreach (GameObject a in AA)
        {
            BB[i] = a.activeInHierarchy;
            i++;
        }
        S1.bb = BB; //Add the 'active list' to our "S1" save state.

        if(checkPoint != null)
        {
            S1.checkPoint = checkPoint.name; //Add the checkPoint gameObject's name to the save state, AS A STRING.
            
        }
        

        FileStream fs = new FileStream("save.dat", FileMode.Create); //Saves to project folder by default.
        BinaryFormatter bf = new BinaryFormatter();
        // Save the 'SaveState' class "S1" and write to a file.
        bf.Serialize(fs, S1);
        fs.Close();
    }
    void LOADGAME()
    {
        using (Stream stream = File.Open("save.dat", FileMode.Open))
        {
            var bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
          
            S1 = (SaveState)bFormatter.Deserialize(stream); //Overrides the current version of "S1" with the saved version.
        }
        
        THANOS_SNAP(); //I couldn't think of a better name TBH.
    }
    void THANOS_SNAP()
    {
        BB = S1.bb;
        checkPoint = GameObject.Find(S1.checkPoint);
        if (checkPoint != null)
        {
            player.transform.position = checkPoint.transform.position;
        }

        int i = 0;
        foreach (GameObject resource in AA)
        {
            resource.SetActive(BB[i]);
            i++;
        }
        
        Loading = false;
        print("Loading Complete");
        
    }


    [System.Serializable]
    public class SaveState
    {
        [SerializeField]
        public List<bool> bb;
        [SerializeField]
        public string checkPoint;
    }

}

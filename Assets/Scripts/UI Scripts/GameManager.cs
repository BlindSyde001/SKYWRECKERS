using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //VARIABLES
    private static GameManager instance;
    public Vector3 lastCheckpointPos;

    //UPDATES
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else {
            Destroy(gameObject); }
    }
    void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        
    }
}

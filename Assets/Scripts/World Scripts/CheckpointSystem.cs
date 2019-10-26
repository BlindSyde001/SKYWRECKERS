using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckpointSystem : MonoBehaviour
{
    //VARIABLES
    private GameManager gm;
    public GameObject dockPos;
    public GameObject checkpointtext;
    public TextMeshProUGUI reachedtext;

    //UPDATES
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    //METHODS
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(savedCheckPoint());
            gm.lastCheckpointPosName = gameObject.name;
            gm.shipLastCheckpointPosName = dockPos.name;
        }
    }
    IEnumerator savedCheckPoint()
    {
        checkpointtext.SetActive(true);
        reachedtext.text = "Checkpoint Saved";
        yield return new WaitForSeconds(3);
        checkpointtext.SetActive(false);
    }
}

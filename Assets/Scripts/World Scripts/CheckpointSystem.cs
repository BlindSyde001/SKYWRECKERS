using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckpointSystem : MonoBehaviour
{
    //VARIABLES
    private GameManager gm;
    public GameObject dockPos;
    public Image checkpointtext;

    //UPDATES
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        checkpointtext.canvasRenderer.SetAlpha(0);
    }
    private void Update()
    {

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
        checkpointtext.canvasRenderer.SetAlpha(1);
        yield return new WaitForSeconds(2);
        checkpointtext.CrossFadeAlpha(0, 3, false);
        yield return null;
    }
}

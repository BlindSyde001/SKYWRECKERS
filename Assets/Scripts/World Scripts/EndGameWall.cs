using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameWall : MonoBehaviour
{
    public GameObject EndGameScreen;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Time.timeScale = 0;
            EndGameScreen.SetActive(true);
        }
    }
}

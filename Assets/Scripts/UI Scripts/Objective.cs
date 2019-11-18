using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Objective : MonoBehaviour
{
    private UIManager UI;
    private bool itemCheck = false;

    private void Awake()
    {
        UI = FindObjectOfType<UIManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(itemCheck == false)
            {
                UI.UIObjectText.text = "Press E to Pick Up";
                if(Input.GetKeyDown(KeyCode.E))
                {
                   UI.objectiveMarkers++;
                   itemCheck = true;
                }
            }
            else
            {
                UI.UIObjectText.text = "";
            }
        }
    }
}

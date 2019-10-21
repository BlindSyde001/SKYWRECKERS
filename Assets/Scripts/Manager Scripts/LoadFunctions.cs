using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class LoadFunctions : MonoBehaviour
{
    //VARIABLES
    public GameManager gm;
    public TextMeshProUGUI saveOne;
    public TextMeshProUGUI saveTwo;
    public TextMeshProUGUI saveThree;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        if(File.Exists("Save File " + 0 + ".dat"))
        {
            saveOne.text = "Saved Game Data";
        }
        else
        {
            saveOne.text = "Empty Save";
        }
        if (File.Exists("Save File " + 1 + ".dat"))
        {
            saveTwo.text = "Saved Game Data";
        }
        else
        {
            saveTwo.text = "Empty Save";
        }
        if (File.Exists("Save File " + 2 + ".dat"))
        {
            saveThree.text = "Saved Game Data";
        }
        else
        {
            saveThree.text = "Empty Save";
        }
    }

    //METHODS
    public void NewGame()
    {
        gm.newGame = true;
        SceneManager.LoadScene(1);
        Time.timeScale = 1.0f;
    }

    public void LoadGame(int slot)
    {
        gm.newGame = false;
        gm.LoadGameFile(slot);
        SceneManager.LoadScene(1);
        Time.timeScale = 1.0f;
    }

    public void LastCheckpoint()
    {
        gm.newGame = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SaveGame(int slot)
    { //Made into a switch in case I need to do anything else.
        switch (slot)
        {
            case 2:
                gm.SaveGameFile(slot);
                saveThree.text = "Saved Game Data";
                break;

            case 1:
                gm.SaveGameFile(slot);
                saveTwo.text = "Saved Game Data";
                break;

            case 0:
                gm.SaveGameFile(slot);
                saveOne.text = "Saved Game Data";
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

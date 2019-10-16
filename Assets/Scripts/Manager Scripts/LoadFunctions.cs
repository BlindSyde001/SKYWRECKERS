using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFunctions : MonoBehaviour
{
    //VARIABLES
    public GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
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
    {
        gm.SaveGameFile(slot);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

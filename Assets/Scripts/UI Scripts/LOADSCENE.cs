using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LOADSCENE : MonoBehaviour
{
    //VARIABLES
    public GameManager gm;


    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }
    //METHODS
    public void startGame()
    {
        if (gm != null)
        { Destroy(gm.gameObject); }

        SceneManager.LoadScene(1);
        gm = FindObjectOfType<GameManager>();
        Time.timeScale = 1.0f;
        Cursor.visible = false;
    }

    public void lastCheckpoint()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Mainmenu()
    {
        SceneManager.LoadScene(0);
    }
}

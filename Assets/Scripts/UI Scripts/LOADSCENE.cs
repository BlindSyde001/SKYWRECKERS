using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LOADSCENE : MonoBehaviour
{
    //VARIABLES
    public GameManager gm;

    //METHODS
    public void startGame()
    {
        gm = FindObjectOfType<GameManager>();
        if (gm != null)
        { Destroy(gm.gameObject); }

        SceneManager.LoadScene(1);
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

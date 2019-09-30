using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LOADSCENE : MonoBehaviour
{
    //METHODS
    public void LoadGame()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1.0f;
        Cursor.visible = false;
    }

    public void mainmenu()
    {
        SceneManager.LoadScene(0);
    }
}

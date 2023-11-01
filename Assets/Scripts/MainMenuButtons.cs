using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
   public void start()
    {
        SceneManager.LoadScene("Level2");
    }

    public void endgame()
    {
        Application.Quit();
    }
}

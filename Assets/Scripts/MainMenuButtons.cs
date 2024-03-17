using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject mainmenubuts;
    public GameObject levelselectbuts;

   public void start()
    {
        SceneManager.LoadScene("PrototypeIntro");
    }

    public void endgame()
    {
        Application.Quit();
    }

    public void triggerbuts()
    {
        mainmenubuts.SetActive(!mainmenubuts.activeInHierarchy);
        levelselectbuts.SetActive(!levelselectbuts.activeInHierarchy);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public Button[] levelButtons;

    void Start()
    {
        UpdateLevelSelectScreen();
        PlayerPrefs.SetInt("Level1Unlocked", 1);
    }

    void UpdateLevelSelectScreen()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (PlayerPrefs.GetInt("Level" + (i + 1) + "Unlocked", 0) == 1)
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Level"+this.gameObject.name);
    }
}

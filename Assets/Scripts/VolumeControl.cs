using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider;
    public TextMeshProUGUI sliderText;
    public bool paused;
    public GameObject menu;
    // Start is called before the first frame update
    void Start()
    {
        volumeSlider = GetComponent<Slider>();
        if (!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 1);
            LoadVolume();
        }
        else
        {
            LoadVolume();
        }

        sliderText.text = "Volume (" + volumeSlider.value *1000 + ")";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
        }
        if (!paused)
        {
            Time.timeScale = 1;
            menu.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            menu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        sliderText.text = "Volume (" + volumeSlider.value * 1000 + ")";
        SaveVolume();
    }

    private void LoadVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }

    public void retryStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

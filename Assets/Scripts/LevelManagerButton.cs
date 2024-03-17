using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManagerButton : MonoBehaviour
{
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(LoadLevel);
    }
    public void LoadLevel()
    {
        Debug.Log("Level" + this.gameObject.name);
        SceneManager.LoadScene("Level" + this.gameObject.name);
    }
}

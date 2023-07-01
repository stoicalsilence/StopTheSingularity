using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Subtitles : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI subtitlesText;
    public List<string> subtitles;
    public List<float> timings;

    private int currentIndex = 0;
    private float timer = 0f;

    private void Start()
    {
        if (subtitles.Count != timings.Count)
        {
            Debug.LogError("The number of subtitles and timings must be the same!");
            return;
        }

        if (subtitlesText == null)
        {
            Debug.LogError("TextMeshProUGUI reference is missing!");
            return;
        }

        // Start showing subtitles
        //StartCoroutine(ShowSubtitles());
    }
    public void startSubtitles()
    {
        StartCoroutine(ShowSubtitles());
    }
    public IEnumerator ShowSubtitles()
    {
        while (currentIndex < subtitles.Count)
        {
            panel.gameObject.SetActive(true);
            subtitlesText.text = subtitles[currentIndex];
            yield return new WaitForSeconds(timings[currentIndex]);
            currentIndex++;
        }
        panel.gameObject.SetActive(false);
        subtitlesText.text = "";
    }
}
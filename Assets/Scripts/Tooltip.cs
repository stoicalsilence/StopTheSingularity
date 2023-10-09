using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI tooltipText;
    

    public void getReportedTo(string textToShow)
    {
        tooltipText.fontSize = 1f;
        tooltipText.gameObject.SetActive(true);
        tooltipText.text = textToShow;
        StartCoroutine(AnimateFontSize());
    }

    private IEnumerator AnimateFontSize()
    {
        float targetFontSize = 50f;
        float animationDuration = 0.25f;

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            tooltipText.fontSize = Mathf.Lerp(1f, targetFontSize, t);
            yield return null;
        }

        // Keep the font size at 64 for 4 seconds second
        yield return new WaitForSeconds(4f);

        // Reduce font size back to 1
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            tooltipText.fontSize = Mathf.Lerp(targetFontSize, 1f, t);
            yield return null;
        }

        turnoffText();
    }

    private void turnoffText()
    {
        tooltipText.gameObject.SetActive(false);
    }
}


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillText : MonoBehaviour
{
    public List<string> stringList = new List<string>();
    public TextMeshProUGUI killText;
    private Coroutine animationCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getReportedTo()
    {
        killText.fontSize = 1f;
        killText.gameObject.SetActive(true);
        int randomIndex = Random.Range(0, stringList.Count);
        string foundText = stringList[randomIndex];
        killText.text = foundText;

        StartCoroutine(AnimateFontSize());
    }

    private IEnumerator AnimateFontSize()
    {
        float targetFontSize = 72f;
        float animationDuration = 0.25f;

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            killText.fontSize = Mathf.Lerp(1f, targetFontSize, t);
            yield return null;
        }

        // Keep the font size at 64 for a second
        yield return new WaitForSeconds(1f);

        // Reduce font size back to 1
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            killText.fontSize = Mathf.Lerp(targetFontSize, 1f, t);
            yield return null;
        }

        turnoffText();
    }

    private void turnoffText()
    {
        killText.gameObject.SetActive(false);
    }
}

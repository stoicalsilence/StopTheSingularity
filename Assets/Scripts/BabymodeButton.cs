using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BabymodeButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    private void Start()
    {
        setText();
    }
    public void toggleButton()
    {
        BabyMode.activated = !BabyMode.activated;
        setText();
    }

    private void setText()
    {
        if (BabyMode.activated)
        {
            buttonText.text = "Babymode ON";
        }
        else
        {
            buttonText.text = "Babymode OFF";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(UpdateHandleSize);
    }

    private void UpdateHandleSize(float value)
    {
        RectTransform handleRect = slider.fillRect;
        handleRect.anchorMax = new Vector2(value, 1f);
    }
}

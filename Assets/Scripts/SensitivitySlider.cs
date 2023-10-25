using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    public Slider slider;
	public TextMeshProUGUI sensitivityText;

	public void Start()
	{
		slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
		slider.value = FindObjectOfType<PlayerCam>().sensitivity;
		sensitivityText.text = "Sensitivity ("+FindObjectOfType<PlayerCam>().sensitivity.ToString()+")";
	}
	public void ValueChangeCheck()
	{
		sensitivityText.text = "Sensitivity ("+slider.value.ToString()+")";
		FindObjectOfType<PlayerCam>().sensitivity = slider.value;
	}
}

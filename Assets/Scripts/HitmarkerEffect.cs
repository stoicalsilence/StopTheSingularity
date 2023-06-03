using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitmarkerEffect : MonoBehaviour
{
    public float duration = 0.2f;

    public RawImage hitmarkerImage;
    private float currentDuration;

    private void Start()
    {
        hitmarkerImage.gameObject.SetActive(false);
    }

    public void ShowHitmarker()
    {
        hitmarkerImage.gameObject.SetActive(true);
        currentDuration = duration;
    }

    private void Update()
    {
        if (hitmarkerImage.enabled)
        {
            currentDuration -= Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, 1f - (currentDuration / duration));

            Color hitmarkerColor = hitmarkerImage.color;
            hitmarkerColor.a = alpha;
            hitmarkerImage.color = hitmarkerColor;

            if (currentDuration <= 0f)
            {
                hitmarkerImage.gameObject.SetActive(false);
            }
        }
    }
}
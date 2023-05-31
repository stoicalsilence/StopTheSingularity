using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake instance;

    private Vector3 originalPosition;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.7f;
    private float dampingSpeed = 1.0f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPosition;
        }
    }

    public static void Shake(float duration, float magnitude)
    {
        instance.shakeDuration = duration;
        instance.shakeMagnitude = magnitude;
    }
}

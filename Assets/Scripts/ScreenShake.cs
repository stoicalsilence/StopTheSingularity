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

    private float smoothShakeDuration = 0f;
    private float smoothShakeMagnitude = 0.5f;
    private float smoothDampingSpeed = 2.0f;

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
            if (shakeMagnitude > 0f)
            {
                shakeMagnitude -= Time.deltaTime / 2;
            }
        }
        else
        {
            shakeDuration = 0f;
            if (smoothShakeDuration <= 0f) // Only reset position if there's no smooth shake active
                transform.localPosition = originalPosition;
        }

        if (smoothShakeDuration > 0)
        {
            float smoothX = Mathf.PerlinNoise(Time.time * smoothDampingSpeed, 0f) - 0.5f;
            float smoothY = Mathf.PerlinNoise(0f, Time.time * smoothDampingSpeed) - 0.5f;
            float smoothZ = Mathf.PerlinNoise(Time.time * smoothDampingSpeed, Time.time * smoothDampingSpeed) - 0.5f;

            Vector3 smoothOffset = new Vector3(smoothX, smoothY, smoothZ) * smoothShakeMagnitude;
            transform.localPosition = originalPosition + smoothOffset;

            smoothShakeDuration -= Time.deltaTime;
        }
        else if (shakeDuration <= 0f) // Only reset position if there's no shake active
        {
            smoothShakeDuration = 0f;
            transform.localPosition = originalPosition;
        }
    }

    public static void Shake(float duration, float magnitude)
    {
        instance.shakeDuration = duration;
        instance.shakeMagnitude = magnitude;
    }

    public static void SmoothShake(float duration, float magnitude)
    {
        instance.smoothShakeDuration = duration;
        instance.smoothShakeMagnitude = magnitude;
    }
}

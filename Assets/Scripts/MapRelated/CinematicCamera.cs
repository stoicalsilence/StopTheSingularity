using System.Collections;
using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // Speed of camera rotation.
    public float maxRotationAngle = 10.0f; // Maximum angle the camera can rotate.
    public float shakeMagnitude = 0.1f; // Magnitude of camera shake.
    public float shakeSpeed = 1.0f; // Speed of camera shake.

    private Quaternion initialRotation;
    private Vector3 initialPosition;

    private void Start()
    {
        // Store the initial rotation and position of the camera.
        initialRotation = transform.rotation;
        initialPosition = transform.position;

        // Start the rotation and shake.
        StartCoroutine(RotateAndShake());
    }

    private IEnumerator RotateAndShake()
    {
        while (true)
        {
            // Calculate the current rotation angle.
            float angle = Mathf.Sin(Time.time * rotationSpeed) * maxRotationAngle;

            // Apply rotation.
            transform.rotation = initialRotation * Quaternion.Euler(0, angle, 0);

            // Apply camera shake.
            float shakeX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) * shakeMagnitude - (shakeMagnitude / 2);
            float shakeY = Mathf.PerlinNoise(0, Time.time * shakeSpeed) * shakeMagnitude - (shakeMagnitude / 2);
            transform.position = initialPosition + new Vector3(shakeX, shakeY, 0);

            yield return null;
        }
    }
}

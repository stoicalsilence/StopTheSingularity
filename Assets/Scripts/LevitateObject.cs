using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitateObject : MonoBehaviour
{
    public float levitationHeight = 0.2f; // The height at which the object should levitate
    public float levitationSpeed = 1f; // The speed at which the object should levitate

    private Vector3 originalPosition; // The original position of the object

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        // Calculate the new position using a sine wave
        Vector3 newPosition = originalPosition + new Vector3(0f, Mathf.Sin(Time.time * levitationSpeed) * levitationHeight, 0f);

        // Update the object's position
        transform.position = newPosition;

        // Rotate the object smoothly
        transform.Rotate(Vector3.up, Time.deltaTime * 30f);
    }
}
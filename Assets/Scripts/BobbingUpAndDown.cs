using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingUpAndDown : MonoBehaviour
{
    public float bobbingSpeed = 1f;  // Speed of the bobbing motion
    public float bobbingHeight = 1f; // Height of the bobbing motion

    private float startY;            // Initial Y position of the object

    void Start()
    {
        startY = transform.position.y; // Store the initial Y position of the object
    }

    void Update()
    {
        // Calculate the new Y position based on a sine wave
        float newY = startY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;

        // Update the object's position with the new Y value
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

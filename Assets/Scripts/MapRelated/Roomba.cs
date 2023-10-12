using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roomba : MonoBehaviour
{
    public GameObject cleaner1, cleaner2;
    public float moveSpeed = 1.0f;  // Speed at which the Roomba moves forward
    public float turnInterval = 3.0f;  // Time interval for turning
    public float turnSpeed = 90.0f;  // Speed at which the Roomba turns

    private float timeSinceLastTurn = 0.0f;
    private bool isTurning = false;
    private Quaternion targetRotation;

    void Update()
    {
        if (isTurning)
        {
            // Interpolate the rotation for smooth turning
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Check if the Roomba has finished turning
            if (transform.rotation == targetRotation)
            {
                isTurning = false;
            }
        }
        else
        {
            // Move the Roomba forward
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Update the time since the last turn
            timeSinceLastTurn += Time.deltaTime;

            // If it's time to turn, choose a random angle and turn
            if (timeSinceLastTurn >= turnInterval)
            {
                float randomTurnAngle = Random.Range(0.0f, 360.0f);
                targetRotation = Quaternion.Euler(0, randomTurnAngle, 0);
                isTurning = true;
                timeSinceLastTurn = 0.0f;  // Reset the timer
            }
        }

        cleaner1.transform.Rotate(Vector3.up * 800 * Time.deltaTime);
        cleaner2.transform.Rotate(Vector3.up * 800 * Time.deltaTime);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewBob : MonoBehaviour
{
    public float bobFrequency = 2f;     // Frequency of the view bob
    public float bobAmount = 0.1f;      // Amount of the view bob
    public float bobSmoothing = 2f;     // Smoothing factor for the view bob

    private float timer = 0f;
    private Vector3 initialPosition;
    public PlayerMovement player;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }//a

    private void Update()
    {
        // Get the player's current speed
        float speed = Mathf.Sqrt(player.horizontalInput * player.horizontalInput + player.verticalInput * player.verticalInput);

        // Calculate the view bob offset
        float bobOffset = Mathf.Sin(timer) * bobAmount * speed;

        // Apply the view bob
        Vector3 targetPosition = initialPosition + new Vector3(0f, bobOffset, 0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, bobSmoothing * Time.deltaTime);

        // Update the timer based on the bob frequency
        timer += bobFrequency * speed * Time.deltaTime;
    }
}
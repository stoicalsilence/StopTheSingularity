using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform playerTransform;
    public float rotationSpeed;

    private void Start()
    {
        // Find the player GameObject or its associated component
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Calculate the direction from this GameObject to the player
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        // Calculate the target rotation to face the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}

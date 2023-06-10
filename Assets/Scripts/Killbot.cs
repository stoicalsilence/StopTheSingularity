using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbot : MonoBehaviour
{
    public Transform player;
    public float damping = 2f;
    public float yRotationOffset = 120f;

    private void Start()
    {
        player = FindObjectOfType<Player>().GetComponent<Transform>();
    }
    

    private void Update()
    {
        // Calculate the direction from the whole object to the player
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        // Calculate the target rotation based on the corrected direction
        Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);

        // Apply a rotation offset to the target rotation
        targetRotation *= Quaternion.Euler(0f, yRotationOffset, 0f);

        // Smoothly rotate the whole object towards the target rotation using damping
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
    }
}

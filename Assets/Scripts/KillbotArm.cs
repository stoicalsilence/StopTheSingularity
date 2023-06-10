using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillbotArm : MonoBehaviour
{
    public Transform player;
    public float damping = 2f;
    private void Start()
    {
        player = FindObjectOfType<Player>().GetComponent<Transform>();
    }
    public float yRotationOffset = 120f;

    private void Update()
    {
        // Calculate the target rotation to look at the player
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);

        // Apply a rotation offset to the target rotation
        targetRotation *= Quaternion.Euler(0f, yRotationOffset, 0f);

        // Smoothly rotate the arm towards the target rotation using damping
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public float moveSpeed = 3f; // Enemy movement speed
    public float detectionRange = 10f; // Range at which the enemy detects the player
    public float roamRadius = 5f; // Radius within which the enemy roams

    private Transform player; // Reference to the player's transform
    private bool isChasing = false; // Flag to indicate if the enemy is currently chasing the player
    private Vector3 randomDestination; // Random destination for roaming behavior

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag (make sure the player has the "Player" tag)
    }

    private void Update()
    {
        if (isChasing)
        {
            // Move towards the player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            // Roaming behavior
            if (Vector3.Distance(transform.position, randomDestination) <= 1f)
            {
                GenerateRandomDestination();
            }

            // Move towards the random destination
            Vector3 direction = (randomDestination - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    private void GenerateRandomDestination()
    {
        // Generate a random position within the roamRadius around the enemy's current position
        randomDestination = transform.position + Random.insideUnitSphere * roamRadius;
        randomDestination.y = transform.position.y; // Make sure the enemy stays at the same height
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = true; // Start chasing the player
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false; // Stop chasing the player
        }
    }
}


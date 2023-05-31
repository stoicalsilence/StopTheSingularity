using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform player;
    public float movementSpeed = 2f;
    public float rotationSpeed = 5f;

    private Animator animator;
    private Rigidbody rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Disable root motion to control the robot's movement manually
        animator.applyRootMotion = false;
    }

    private void Update()
    {
        // Calculate direction to the player
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f; // Ignore vertical component

        // Rotate the upper body towards the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Calculate the movement direction based on the player's position
        Vector3 movementDirection = directionToPlayer.normalized;

        // Calculate the target position to move towards
        Vector3 targetPosition = transform.position + (movementDirection * movementSpeed * Time.deltaTime);

        // Move the robot towards the target position using interpolation
        rb.MovePosition(targetPosition);

        // Calculate the walk cycle speed based on the movement speed
        float walkCycleSpeed = movementSpeed / 2f;
        animator.SetFloat("WalkSpeed", walkCycleSpeed);
    }
}
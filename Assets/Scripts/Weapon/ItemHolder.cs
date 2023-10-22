using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Transform camTransform; // Reference to the camera's transform
    public float distance = 2f; // Distance from the camera
    public Vector3 offset = Vector3.zero; // Offset from the camera's position

    private Vector3 targetOffset; // Target offset based on the value of player.isAttackingWithPlasmatana
    private float offsetSpeed = 5f; // Speed at which the offset transitions
    public Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void LateUpdate()
    {
        // Calculate the desired position based on the camera's position and rotation
        Vector3 desiredPosition = camTransform.position + camTransform.forward * distance;

        // Smoothly transition the offset based on the value of player.isAttackingWithPlasmatana
        if (player.attackingWithPlasmatana)
        {
            targetOffset = new Vector3(0.5f, -0.7f, -1f);
        }
        else
        {
            targetOffset = new Vector3(0.5f, -0.5f, -1f);
        }

        offset = Vector3.Lerp(offset, targetOffset, offsetSpeed * Time.deltaTime);

        // Apply the offset to the desired position
        desiredPosition += camTransform.rotation * offset;

        // Set the item holder's position to the desired position
        transform.position = desiredPosition;

        // Make the item holder face the same direction as the camera
        transform.rotation = camTransform.rotation;
    }
}

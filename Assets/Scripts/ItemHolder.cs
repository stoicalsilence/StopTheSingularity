using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Transform camTransform; // Reference to the camera's transform
    public float distance = 2f; // Distance from the camera
    public Vector3 offset = Vector3.zero; // Offset from the camera's position

    private void LateUpdate()
    {
        // Calculate the desired position based on the camera's position and rotation
        Vector3 desiredPosition = camTransform.position + camTransform.forward * distance;

        // Apply the offset to the desired position
        desiredPosition += camTransform.rotation * offset;

        // Set the item holder's position to the desired position
        transform.position = desiredPosition;

        // Make the item holder face the same direction as the camera
        transform.rotation = camTransform.rotation;
    }
}

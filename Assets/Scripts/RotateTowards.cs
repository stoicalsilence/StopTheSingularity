using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    public Transform target; // The target object to rotate towards

    void Update()
    {
        if (target != null)
        {
            // Look at the target while keeping the current y-rotation
            transform.LookAt(target.position, Vector3.up);

            // Extract the y-rotation from the current rotation
            float currentYRotation = transform.eulerAngles.y;

            // Apply only the y-rotation to the object
            transform.rotation = Quaternion.Euler(0f, -currentYRotation, 0f);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpwardForcer : MonoBehaviour
{
    public float forceMagnitude = 10f; // Magnitude of the upward force

    private Rigidbody rb; // Reference to the Rigidbody component
    public bool pulling;

    void Start()
    {
        // Get the reference to the Rigidbody component attached to the GameObject
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (pulling)
        {
            // Apply a constant upward force relative to the world's up direction
            Vector3 upwardForce = Vector3.up * forceMagnitude;
            rb.AddForce(upwardForce, ForceMode.Force);
        }
    }
}

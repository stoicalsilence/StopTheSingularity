using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenElevatorTimed : MonoBehaviour
{
    public bool triggered = false;
    public BoxCollider trigger;
    public float time;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            trigger.enabled = false;
            Invoke("openElevator", time);
        }
    }
    public void openElevator()
    {

        FindObjectOfType<Elevator>().opened = true;
    }
}

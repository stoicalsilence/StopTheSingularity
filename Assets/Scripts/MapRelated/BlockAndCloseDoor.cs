using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAndCloseDoor : MonoBehaviour
{
    public bool blocking;
    public BoxCollider collider;
    public ButtonManipulation door;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            blocking = true;
            collider.enabled = true;
            if (door != null) door.openDoorButton.isPressed = false;
        }
    }
}

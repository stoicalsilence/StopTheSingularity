using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLaserDoor : MonoBehaviour
{
    public GameObject barrier;
    public OpenDoorButton openDoorButton;

    private void Update()
    {
        if (openDoorButton.isPressed)
        {
            Destroy(barrier.gameObject);
        }
    }
}

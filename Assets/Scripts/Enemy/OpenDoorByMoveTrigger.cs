using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorByMoveTrigger : MonoBehaviour
{
    public OpenDoorButton op;
    public AudioSource audiosource;

    private void OnTriggerEnter(Collider other)
    {
        op.isPressed = true;
        audiosource.Play();
        Destroy(this);
    }
}

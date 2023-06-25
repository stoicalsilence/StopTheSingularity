using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launchpad : MonoBehaviour
{
    public float launchForce = 10f; // Adjust this value to control the launch force.
    public AudioSource audioSource;
    public AudioClip[] jumpSounds;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "PlayerAttack")
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {

                int randomIndex = Random.Range(0, jumpSounds.Length);
                AudioClip jumpSound = jumpSounds[randomIndex];
                audioSource.clip = jumpSound;
                audioSource.PlayOneShot(audioSource.clip);
                rb.AddForce(transform.up * launchForce, ForceMode.Impulse);
                if (rb.gameObject.GetComponent<Killbot>())
                {
                    rb.gameObject.GetComponent<Killbot>().turnOffNavMeshAgent();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.up * launchForce/3, ForceMode.Impulse);
        }
    }
}
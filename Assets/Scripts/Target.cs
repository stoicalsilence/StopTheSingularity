using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject breakParticles;
    public AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            GameObject br = Instantiate(breakParticles, transform.position, Quaternion.identity);
            Destroy(br, 4f);

            audioSource.gameObject.transform.SetParent(null);
            audioSource.PlayOneShot(audioSource.clip);
            Destroy(audioSource.gameObject,4f);
            Destroy(this.gameObject);
        }
    }
}
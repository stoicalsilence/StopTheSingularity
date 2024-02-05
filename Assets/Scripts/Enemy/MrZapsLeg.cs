using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrZapsLeg : MonoBehaviour
{

    public LayerMask groundLayer;

    public AudioSource source;
    public AudioClip step;
    public GameObject particles;
    // Start is called before the first frame update


    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider is on the specified layer
        if (groundLayer == (groundLayer | (1 << other.gameObject.layer)))
        {
            source.PlayOneShot(step);
            GameObject obj = Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(obj, 4f);
        }
    }
}

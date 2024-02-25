using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyojunFoot : MonoBehaviour
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
            float distanceToPlayer = Vector3.Distance(transform.position, FindObjectOfType<Player>().transform.position);
            source.PlayOneShot(step);
            GameObject obj = Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(obj, 4f);
            if(distanceToPlayer < 10)
            {
                ScreenShake.Shake(0.45f, 0.4f);
            }
            else if (distanceToPlayer < 30 && distanceToPlayer > 10)
            {
                ScreenShake.Shake(0.15f, 0.3f);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionForce = 10f;  // Force applied to objects within the explosion radius
    public float explosionRadius = 5f;  // Radius within which objects are affected by the explosion
    public GameObject explosionParticles, orangeLight;

    private void Start()
    {
        Explode();
        GameObject a = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Destroy(a, 4);
        GameObject b = Instantiate(orangeLight, transform.position, Quaternion.identity);
        Destroy(b, 0.5f);
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Destroy the explosion game object after the explosion
        Destroy(gameObject);
    }
}
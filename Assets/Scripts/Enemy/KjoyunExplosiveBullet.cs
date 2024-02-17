using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KjoyunExplosiveBullet : MonoBehaviour
{
    public float explosionRadius;
    public float explosionForce;
    public GameObject particles;


    private void OnCollisionEnter(Collision collision)
    {

        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var obj in surroundingObjects)
        {
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            if (obj.GetComponent<Player>())
            {
                obj.GetComponent<Player>().takeDamage(5);
            }
        }

        GameObject part = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(part, 5f);
        Destroy(gameObject);
    }
}

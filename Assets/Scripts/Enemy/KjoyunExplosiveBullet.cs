using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KjoyunExplosiveBullet : MonoBehaviour
{
    public float explosionRadius;
    public float explosionForce;
    public GameObject particles;
    public bool isGetty;


    private void OnCollisionEnter(Collision collision)
    {

        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var obj in surroundingObjects)
        {
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            if (obj.GetComponent<Player>())
            {
                if (!isGetty)
                {
                    obj.GetComponent<Player>().takeDamage(5);
                }
                else
                {
                    obj.GetComponent<Player>().takeDamage(2);
                }
            }

            if (obj.GetComponent<KyojunPillar>())
            {
                obj.GetComponent<KyojunPillar>().getBroken();
            }
        }

        GameObject part = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(part, 5f);
        Destroy(gameObject);
    }
}

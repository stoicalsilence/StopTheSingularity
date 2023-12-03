using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownGrenade : MonoBehaviour
{
    public GameObject explosion1, explosion2, hitParticles;
    public float timeToExplode, explosionForce, explosionRadius;
    public AudioSource audioSource;
    public AudioClip throwSound, impactSound;

    void Start()
    {
        audioSource.PlayOneShot(throwSound);
        Invoke("explode", timeToExplode);
    }

    public void explode()
    {
        Collider[] sorroundingObjects;
        sorroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var obj in sorroundingObjects)
        {
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            var en = obj.GetComponent<Eyenemy>();
            if (en != null)
            {
                int amodmg = en.health;
                for (int i = 0; i < amodmg; i++)
                {
                    en.TakeDamage();
                }
                FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
            }
            var en2 = obj.GetComponent<Killbot>();
            if (en2 != null)
            {
                int amodmg = en2.health;
                for (int i = 0; i < amodmg; i++)
                {
                    en2.takeDamage();
                }
            }
            var en3 = obj.GetComponent<FlyingPuter>();
            if (en3 != null)
            {
                int amodmg = en3.health;
                for (int i = 0; i < amodmg; i++)
                {
                    en3.takeDamage();
                }
            }
            var en4 = obj.GetComponent<PuterTurret>();
            if (en4 != null)
            {
                int amodmg = en4.health;
                for (int i = 0; i < amodmg; i++)
                {
                    en4.takeDamage();
                }
            }
            var en5 = obj.GetComponent<RollingEnemy>();
            if(en5 != null)
            {
                int amodmg = en5.health;
                for (int i = 0; i < amodmg; i++)
                {
                    en5.TakeDamage();
                }
            }

            var dest = obj.GetComponent<Destructable>();
            if (dest != null) { dest.spawnDestroyParticles(); dest.explode(); }

            //var barrel = obj.GetComponent<ExplosiveBarrel>();
            //if (barrel != null && barrel != this) { barrel.explode(); Destroy(barrel); }

            var trgt = obj.GetComponent<Target>();
            if (trgt != null) { trgt.explode(); }

            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        GameObject expo = Instantiate(explosion1, transform.position, Quaternion.identity);
        GameObject expo2 = Instantiate(explosion2, transform.position, Quaternion.identity);
        Destroy(expo, 6f);
        Destroy(expo2, 6f);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionPoint = collision.GetContact(0).point;

        GameObject impactFX = Instantiate(hitParticles, collisionPoint, Quaternion.identity);
        Destroy(impactFX, 5f);

        audioSource.PlayOneShot(impactSound);
    }
}

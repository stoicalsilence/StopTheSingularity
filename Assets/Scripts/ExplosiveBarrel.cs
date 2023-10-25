using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public float explosionRadius;
    public float explosionForce;
    public GameObject particles;
    public GameObject newparticles;
    Collider[] sorroundingObjects;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAttack" || collision.gameObject.name =="EnemyBullet(Clone)")
        {
            explode();
        }
    }
    public void explode()
    {
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
            var dest = obj.GetComponent<Destructable>();
            if (dest != null) { dest.spawnDestroyParticles(); dest.explode(); }

            var barrel = obj.GetComponent<ExplosiveBarrel>();
            if (barrel != null) { barrel.explode(); }

            var trgt = obj.GetComponent<Target>();
            if (trgt != null) { trgt.explode(); }

            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        GameObject part = Instantiate(particles, transform.position, Quaternion.identity);
        GameObject part2 = Instantiate(newparticles, transform.position, Quaternion.identity);
        Destroy(part, 5f);
        Destroy(part2, 5f);
        Destroy(gameObject);
    }
}

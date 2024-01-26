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
    public int playerpushforce;

    private void Start()
    {
        if(playerpushforce == 0)
        {
            playerpushforce = 6;
        }
    }
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
            var en5 = obj.GetComponent<RollingEnemy>();
            if (en5 != null)
            {
                int amodmg = en5.health;
                for (int i = 0; i < amodmg; i++)
                {
                    en5.TakeDamage();
                }
            }

            var en6 = obj.GetComponent<Player>();
            if(en6 != null)
            {
                Vector3 pushDirection = this.transform.position - transform.position;
                pushDirection.y = Random.Range(2,4);

                pushDirection = pushDirection.normalized;

                FindObjectOfType<Player>().gameObject.GetComponent<Rigidbody>().AddForce(pushDirection * playerpushforce, ForceMode.Impulse);
            }

            //var barrel = obj.GetComponent<ExplosiveBarrel>();
            //if (barrel != null && barrel != this) { barrel.explode(); Destroy(barrel); }

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


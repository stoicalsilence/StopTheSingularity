using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    public float explosionRadius;
    public float explosionForce;
    public GameObject particles;
    

    private void OnCollisionEnter(Collision collision)
    {
        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach(var obj in surroundingObjects)
        {
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            var en = obj.GetComponent<Eyenemy>();
            if (en != null) { en.TakeDamage(); FindObjectOfType<HitmarkerEffect>().ShowHitmarker(); }
            var en2 = obj.GetComponent<Killbot>();
            if (en2 != null) { en2.takeDamage(); FindObjectOfType<HitmarkerEffect>().ShowHitmarker(); }
            var en3 = obj.GetComponent<FlyingPuter>();
            if(en3 != null) { en3.takeDamage(); FindObjectOfType<HitmarkerEffect>().ShowHitmarker(); }
            var en4 = obj.GetComponent<PuterTurret>();
            if(en4 != null) { en4.takeDamage(); }
            var dest = obj.GetComponent<Destructable>();
            if(dest != null) { dest.spawnDestroyParticles(); dest.explode(); }
            var trgt = obj.GetComponent<Target>();
            if(trgt != null) { trgt.explode(); }
            var en5 = obj.GetComponent<RollingEnemy>();
            if (en5 != null)
            {
                int amodmg = en5.health;
                for (int i = 0; i < amodmg; i++)
                {
                    en5.TakeDamage();
                }
            }
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        GameObject part = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(part, 5f);
        Destroy(gameObject);
    }
}

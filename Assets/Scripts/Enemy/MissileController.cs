using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    [Header("Missile")]
    public float missileSpeed = 10f;
    public float rotationSpeed = 5f;
    public Transform target;

    [Header("Explosion")]
    public float explosionRadius;
    public float explosionForce;
    public GameObject particles;
    public GameObject explosionSound;

    public GameObject missile;
    public float selfexplodetimer;
    public bool shouldHurtPlayer;

    public bool returnToPutey;
    public Transform puteyBoss;
    private void Start()
    {
        Invoke("explode", selfexplodetimer);
    }
    private void Update()
    {
        if (target == null)
        {
            // No target to seek, so just keep moving forward
            transform.Translate(Vector3.forward * missileSpeed * Time.deltaTime);
        }
        else
        {
            /// Calculate the direction to the target
            Vector3 targetDirection = target.position - transform.position;

            // Rotate the missile to face the target
            Quaternion rotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

            // Move the missile forward
            transform.Translate(Vector3.forward * missileSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "PlayerAttack" || collision.gameObject.tag != "AntiRocketGel")
        {
            explode();
        }
        else
        {
            shouldHurtPlayer = false;
            if (returnToPutey)
            {
                target = puteyBoss;
            }
        }

        if (collision.gameObject.tag == "PlayerAttack" && !returnToPutey)
        {
            shouldHurtPlayer = false;
            explode();
        }
    }

    public void explode()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer < 30)
        {
            ScreenShake.Shake(0.7f, 0.4f);
        }

        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var obj in surroundingObjects)
        {
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            var en = obj.GetComponent<Eyenemy>();
            if (en != null) { en.TakeDamage(); FindObjectOfType<HitmarkerEffect>().ShowHitmarker(); }
            var en2 = obj.GetComponent<Killbot>();
            if (en2 != null) { en2.takeDamage(); FindObjectOfType<HitmarkerEffect>().ShowHitmarker(); }
            var en3 = obj.GetComponent<FlyingPuter>();
            if (en3 != null) { en3.takeDamage(); FindObjectOfType<HitmarkerEffect>().ShowHitmarker(); }
            var en4 = obj.GetComponent<PuterTurret>();
            if (en4 != null) { en4.takeDamage(); }
            var dest = obj.GetComponent<Destructable>();
            if (dest != null) { dest.spawnDestroyParticles(); dest.explode(); }
            var trgt = obj.GetComponent<Target>();
            if (trgt != null) { trgt.explode(); }
            if(shouldHurtPlayer) { var player = obj.GetComponent<Player>(); if(player != null) player.takeDamage(10);  }

            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        GameObject part = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(part, 5f);
        GameObject explosionsound = Instantiate(explosionSound, transform.position, Quaternion.identity);
        Destroy(explosionsound, 5f);
        Destroy(gameObject);
    }
}

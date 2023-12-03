using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemy : MonoBehaviour
{
    public Transform target;
    public float rollSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpDistanceThreshold = 2f;
    public float detectionRange;

    private Rigidbody rb;
    private RaycastHit hitInfo;
    private Transform player;
    public bool jumpOnCd;
    public Vector3 jumpDirection;
    public AudioSource audiosource, rollingsource, tickingsource;
    public AudioClip jumpTelegraph, rollingSound, death, boing, ticking;
    public AudioClip[] passiveSounds;
    
    public GameObject landParticles, explosionParticles;
    public AudioClip[] thudSounds, shockSounds;
    public int health = 2;
    bool dead;
    public GameObject sparkParticles;
    public GameObject debris;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>().transform;
        InvokeRepeating("PlayPassiveSounds", 0.1f, 0.25f);
    }

    void Update()
    {
        if (dead) return;
        if (target == null)
        {
            if (Physics.Raycast(transform.position, player.position - transform.position, out hitInfo, detectionRange))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                    getTriggered();
                }
            }
            return;
        }

        Vector3 moveDirection = (target.position - transform.position).normalized;

        rb.AddForce(moveDirection * rollSpeed * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer <= jumpDistanceThreshold && !jumpOnCd)
        {
            jumpDirection = (target.position - transform.position).normalized;
            JumpTowardsTarget();
        }

        if (IsOnGround())
        {
            PlayRollingSound();
        }
        //PlayWhrringSound();
    }

    void playThudSound()
    {
        int c3 = Random.Range(0, thudSounds.Length);
        AudioClip c2 = thudSounds[c3];
        audiosource.PlayOneShot(c2);
    }

   public void TakeDamage()
    {
        if (!target)
        {
            getTriggered();
        }
        health--;
        playThudSound();
        if(health < 1)
        {
            Die();
        }
        GameObject spark = Instantiate(sparkParticles, transform.position, Quaternion.identity);
        Destroy(spark, 4f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerAttack"))
        {
            if(health > 0)
            TakeDamage();
            FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("whatisGround"))
        {
            Vector3 collisionPoint = collision.GetContact(0).point;
            GameObject jp = Instantiate(landParticles, collisionPoint, Quaternion.identity);
            Destroy(jp, 4f);
            playThudSound();
        }

        if (collision.collider.CompareTag("Player"))
        {
            Vector3 direction = transform.position - collision.transform.position;
            direction.y = 0f;


            float jumpForce = Mathf.Sqrt(2f * Physics.gravity.magnitude * 3);

            rb.AddForce(direction.normalized * jumpForce, ForceMode.VelocityChange);
            playShockSound();
        }
    }

    void JumpTowardsTarget()
    {
        Invoke("Jump", 0.5f);
        audiosource.PlayOneShot(jumpTelegraph);
        jumpOnCd = true;
        Invoke("ResetJumpCd", 3f);
    }

    void Jump()
    {
        rb.AddForce(jumpDirection.normalized * jumpForce, ForceMode.Impulse);
        audiosource.PlayOneShot(boing);
    }

    void Die()
    {
        dead = true;
        target = null;
        audiosource.PlayOneShot(death);
        Invoke("Explode", 1);
    }
    
    void Explode()
    {
        GameObject p = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Destroy(p, 4);
        for (int i = 0; i < 4; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * 2.0f;
            Vector3 spawnPosition = transform.position + randomOffset;
            GameObject debrisObject = Instantiate(debris, spawnPosition, Quaternion.identity);
            Destroy(debrisObject, 6f);
            Vector3 randomRotation = new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
            debrisObject.transform.rotation = Quaternion.Euler(randomRotation);

            Rigidbody debrisRigidbody = debrisObject.GetComponent<Rigidbody>();
            if (debrisRigidbody != null)
            {
                float launchForce = 10.0f;
                Vector3 launchDirection = (debrisObject.transform.position - transform.position).normalized;
                debrisRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

                Vector3 torque = Random.insideUnitSphere * 5.0f;
                debrisRigidbody.AddTorque(torque, ForceMode.Impulse);
            }
        }
        Destroy(this.gameObject);
    }
    void ResetJumpCd()
    {
        jumpOnCd = false;
    }

    public void getTriggered()
    {
        target = player;
        audiosource.PlayOneShot(jumpTelegraph);
    }

    bool IsOnGround()
    {
        RaycastHit groundHit;
        float raycastDistance = 1.0f;

        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, raycastDistance, LayerMask.GetMask("whatisGround")))
        {
            return true;
        }

        return false;
    }

    void PlayRollingSound()
    {
        if (!rollingsource.isPlaying)
        {
            rollingsource.PlayOneShot(rollingSound);
        }
    }

    void PlayWhrringSound()
    {
        if (!tickingsource.isPlaying)
        {
            tickingsource.PlayOneShot(ticking);
        }
    }
    private void PlayPassiveSounds()
    {
        if (!audiosource.isPlaying)
        {
            int randomIndex = Random.Range(0, passiveSounds.Length);
            AudioClip sound = passiveSounds[randomIndex];
            audiosource.PlayOneShot(sound);
        }
    }

    void playShockSound()
    {
        int randomIndex = Random.Range(0, shockSounds.Length);
        AudioClip sound = shockSounds[randomIndex];
        audiosource.PlayOneShot(sound);
    }
}
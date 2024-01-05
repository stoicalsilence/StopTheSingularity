using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Killbot : MonoBehaviour
{
    public int health = 7;
    public Transform player, bullethole;
    public float damping = 2f;
    public float yRotationOffset = 120f;
    public bool triggered;
    public float detectionRange = 10f;
    public float minimumRange = 3f;
    public float movementSpeed = 5f; // New speed variable
    public Animator animator;
    public bool incover = false;
    public ParticleSystem muzzleFlare;
    public GameObject orangeLight, bulletPrefab;

    public float shootInterval = 1.0f;
    public float bulletSpeed = 10.0f;
    public float bulletInaccuracy = 5.0f;

    private float shootTimer = 0.0f;

    public AudioSource audioSource;
    public AudioSource hitSounds;
    public AudioClip[] gunShots;
    public AudioClip[] damageSounds;
    public AudioClip[] dieSounds;
    public AudioClip[] passiveSounds;
    public AudioClip targetedSound;
    public AudioClip letsGetItOn;

    private RaycastHit hitInfo;

    public Collider bodyHitbox;
    public Collider headHitbox;
    public GameObject collisionParticles;
    public GameObject explosionParticles;

    public AudioSource footstepFX;
    public AudioClip[] footsteps;
    private float lastFootstepTime;
    public Rigidbody rb;
    public GameObject footstepParticles;

    public NavMeshAgent agent;
    bool ded;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("PlayPassiveSounds", 0.1f, 5);
        player = FindObjectOfType<Player>().transform;
        animator = GetComponent<Animator>();
        animator.SetBool("Idle", true);

        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        agent.updateRotation = false;
    }

    private void Update()
    {
        if (!triggered)
        {
            Vector3 coverbonus = new Vector3(0, 0.5f, 0);
            if (incover) coverbonus = new Vector3(0, 1, 0f);
            if (Physics.Raycast(transform.position + coverbonus, player.position - transform.position, out hitInfo, detectionRange))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                    getTriggered();
                }
            }
        }
        else
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;

            // Check if there is a line of sight to the player
            if (CheckLineOfSightToPlayer())
            {
                // If there's line of sight, attack or run towards the player as needed
                AttackOrRunTowardsPlayer();
            }
            else
            {
                // If there's no line of sight, continue running towards the player
                RunTowardsPlayer();
            }
        }
    }

    private bool CheckLineOfSightToPlayer()
    {
        RaycastHit lineOfSightHit;
        Vector3 playerDirection = player.position - transform.position;
        if (Physics.Raycast(transform.position, playerDirection, out lineOfSightHit, Mathf.Infinity))
        {
            if (lineOfSightHit.collider.CompareTag("Player"))
            {
                // There is a line of sight to the player
                return true;
            }
        }
        // No line of sight to the player
        return false;
    }

    private void AttackOrRunTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
        targetRotation *= Quaternion.Euler(0f, yRotationOffset, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < minimumRange)
        {
            if (agent.enabled)
                agent.isStopped = true;
            animator.SetBool("AttackingStanding", true);
            animator.SetBool("Attacking", false);
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                muzzleFlare.Play();
                ShootBullet();
                shootTimer = 0.0f;
            }
        }
        else
        {
            RunTowardsPlayer();
        }
    }

    private void RunTowardsPlayer()
    {
        if (agent.enabled)
            agent.isStopped = false;
        animator.SetBool("AttackingStanding", false);
        animator.SetBool("Attacking", true);
        if (agent.enabled)
        {
            agent.SetDestination(player.position);
        }
        float footstepInterval = 0.1f / rb.velocity.magnitude;
        float timeSinceLastFootstep = Time.time - lastFootstepTime;

        if (timeSinceLastFootstep >= footstepInterval)
        {
            AudioClip footstepSound = footsteps[Random.Range(0, footsteps.Length)];
            footstepFX.PlayOneShot(footstepSound);
            GameObject step = Instantiate(footstepParticles, footstepFX.transform.position, Quaternion.identity);
            Destroy(step, 3f);
            lastFootstepTime = Time.time;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!ded)
        {
            if (collision.gameObject.CompareTag("PlayerAttack"))
            {
                takeDamage();
                if (health > 0)
                {
                    if (!triggered && animator)
                    {
                        getTriggered();
                    }
                    if(audioSource)
                    audioSource.PlayOneShot(damageSounds[0]);
                    Vector3 collisionPoint = collision.GetContact(0).point;
                    GameObject ded = Instantiate(collisionParticles, collisionPoint, Quaternion.identity);
                    Destroy(ded, 5f);
                }
                else
                {
                    rb.velocity = new Vector2(0, 0);
                    ded = true;
                    int randomIndex = Random.Range(0, dieSounds.Length);
                    AudioClip hitSound = dieSounds[randomIndex];
                    audioSource.clip = hitSound;
                    audioSource.PlayOneShot(audioSource.clip);
                    Vector3 collisionPoint = collision.GetContact(0).point;
                    GameObject oof = Instantiate(explosionParticles, collisionPoint, Quaternion.identity);
                    Destroy(oof, 5f);
                    GameObject lighty = Instantiate(orangeLight, collisionPoint, Quaternion.identity);
                    Destroy(lighty, 0.25f);
                    FindObjectOfType<KillText>().getReportedTo();
                    ScreenShake.Shake(0.25f, 0.05f);
                    Destroy(bodyHitbox);
                    Destroy(headHitbox);
                    Destroy(this.gameObject, 4.2f);
                    Destroy(this); // add change to ragdoll
                    Animator animator = GetComponent<Animator>();
                    Destroy(animator);
                    orangeLight.gameObject.SetActive(false);
                    // Add rigidbody to each child GameObject and apply random torqued force
                    foreach (Transform child in transform)
                    {
                        Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();
                        PhysicMaterial physicMaterial = new PhysicMaterial();
                        physicMaterial.bounciness = 0.1f;
                        if (child.gameObject.activeInHierarchy)
                        {
                            Destroy(child.gameObject, 4f);

                            Vector3 randomForce = Random.onUnitSphere * Random.Range(2f, 5f);
                            Vector3 randomTorque = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

                            if (childRigidbody)
                            {
                                childRigidbody.AddForce(randomForce, ForceMode.Impulse);
                                childRigidbody.AddTorque(randomTorque, ForceMode.Impulse);
                            }
                            BoxCollider boxCollider = child.gameObject.GetComponent<BoxCollider>();
                            if (boxCollider != null)
                            {
                                boxCollider.material = physicMaterial;
                                boxCollider.enabled = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public void takeDamage()
    {
        health--;
        FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
        if (health < 1) { 
        ded = true;
        int randomIndex = Random.Range(0, dieSounds.Length);
        AudioClip hitSound = dieSounds[randomIndex];
        audioSource.clip = hitSound;
        audioSource.PlayOneShot(audioSource.clip);
        
        GameObject oof = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Destroy(oof, 5f);
        GameObject lighty = Instantiate(orangeLight, transform.position, Quaternion.identity);
        Destroy(lighty, 0.25f);
        FindObjectOfType<KillText>().getReportedTo();
        ScreenShake.Shake(0.25f, 0.05f);
        Destroy(bodyHitbox);
        Destroy(headHitbox);
        Destroy(this.gameObject, 4.2f);
        Destroy(this); // add change to ragdoll
        Animator animator = GetComponent<Animator>();
        Destroy(animator);
        orangeLight.gameObject.SetActive(false);
            // Add rigidbody to each child GameObject and apply random torqued force
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();
                    Destroy(child.gameObject, 4f);

                    Vector3 randomForce = Random.onUnitSphere * Random.Range(2f, 5f);
                    Vector3 randomTorque = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

                    childRigidbody.AddForce(randomForce, ForceMode.Impulse);
                    childRigidbody.AddTorque(randomTorque, ForceMode.Impulse);

                    BoxCollider boxCollider = child.gameObject.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {
                        boxCollider.enabled = true;
                    }
                }
            }
        }
    }

    private void PlayPassiveSounds()
    {
       
            if (!audioSource.isPlaying)
            {
                int randomIndex = Random.Range(0, passiveSounds.Length);
                AudioClip sound = passiveSounds[randomIndex];
                audioSource.PlayOneShot(sound);
            }
        
    }

    public void turnOnNavMeshAgent()
    {
        agent.enabled = true;
    }
    public void turnOffNavMeshAgent()
    {
        agent.enabled = false;
    }
    public void toggleNavMeshAgent(float duration)
    {
        agent.enabled = false;
        Invoke("turnOnNavMeshAgent", duration);
    }

    public void getTriggered()
    {
        audioSource.PlayOneShot(targetedSound);
        if (Random.value < 0.25f)
        {
            audioSource.PlayOneShot(letsGetItOn);
        }
        triggered = true;
        animator.SetBool("Idle", false);
        animator.SetBool("Attacking", true);
    }
    public void takeCritDamage()
    {
        Debug.Log("headhit");
    }

    private void ShootBullet()
    {
        StartCoroutine(bulletLight());
        int randomIndex = Random.Range(0, gunShots.Length);
        AudioClip sound = gunShots[randomIndex];
        audioSource.PlayOneShot(sound);
        Vector3 directionToPlayer = player.position - bullethole.position;
        directionToPlayer = ApplyBulletInaccuracy(directionToPlayer);

        GameObject bulletObject = Instantiate(bulletPrefab, bullethole.position, Quaternion.identity);
        Destroy(bulletObject, 3f);
        Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();

        // Set the bullet's velocity based on the direction and speed
        bulletRigidbody.velocity = directionToPlayer.normalized * bulletSpeed;
    }

    public IEnumerator bulletLight()
    {
        orangeLight.SetActive(true);
        yield return new WaitForSeconds(0.16f);
        orangeLight.SetActive(false);
    }
    private Vector3 ApplyBulletInaccuracy(Vector3 direction)
    {
        // Calculate random inaccuracy angles for horizontal and vertical directions
        float horizontalInaccuracyAngle = Random.Range(-bulletInaccuracy, bulletInaccuracy);
        float verticalInaccuracyAngle = Random.Range(-bulletInaccuracy, bulletInaccuracy);

        // Apply the horizontal inaccuracy by rotating the direction vector around the up axis
        Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalInaccuracyAngle, Vector3.up);
        direction = horizontalRotation * direction;

        // Apply the vertical inaccuracy by rotating the direction vector around the right axis
        Quaternion verticalRotation = Quaternion.AngleAxis(verticalInaccuracyAngle, Vector3.right);
        direction = verticalRotation * direction;

        return direction;
    }
}
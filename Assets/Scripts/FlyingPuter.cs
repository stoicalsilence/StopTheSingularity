using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPuter : MonoBehaviour
{
    public Transform player, muzzle1, muzzle2;
    public float flySpeed = 5f;
    public float rotationSpeed = 5f; // Added rotation speed
    public float detectionRange = 10f;
    public float shootingRange = 5f;
    public float shootInterval = 1f;
    public float bulletSpeed = 10f;
    public float bulletInaccuracy = 0.1f; // Added bullet inaccuracy parameter
    public GameObject bulletPrefab, collisionParticles, explosionParticles, orangeLight;
    public float wanderForceMin = 1f;
    public float wanderForceMax = 5f;
    public float maxChangeInterval = 5f; // Maximum interval for changing movement direction
    public float changeDirectionTimer = 0f; // Timer for changing movement direction
    private Vector3 currentDirection; // Current movement direction
    public int health;
    private Rigidbody rb;
    private bool isTriggered;
    private bool isShooting;
    private float shootTimer;
    public bool wandering;
    bool ded;
    public float minTimeBetweenShots = 0.1f;  // Adjust this value to set the minimum duration between individual shots
    public float maxTimeBetweenShots = 0.3f;  // Adjust this value to set the maximum duration between individual shots
    public float minTimeBetweenBursts = 2f;   // Adjust this value to set the minimum duration between bursts of fire
    public float maxTimeBetweenBursts = 4f;   // Adjust this value to set the maximum duration between bursts of fire
    private float nextShotTime;
    private float nextBurstTime;
    public AudioSource audioSource;
    public AudioSource crashtalker;
    public AudioSource hitSounds;
    public AudioClip targetedSound;
    public AudioClip letsGetItOn;
    public AudioClip[] gunShots;
    public AudioClip[] crashTalk;
    public AudioClip[] damageSounds;
    public AudioClip[] dieSounds;
    public AudioClip[] passiveSounds;
    public AudioClip[] thudSounds;
    public GameObject thudParticles;
    public ParticleSystem muzzleFlare;
    public ParticleSystem muzzleFlare2;
    private void Start()
    {
        InvokeRepeating("PlayPassiveSounds", 0.1f, 5);
        wandering = true;
        rb = GetComponent<Rigidbody>();
        isTriggered = false;
        isShooting = false;
        shootTimer = 0f;
        player = FindObjectOfType<Player>().transform;
    }
    private void PlayPassiveSounds()
    {

        if (!audioSource.isPlaying)
        {
            int randomIndex = Random.Range(0, passiveSounds.Length);
            AudioClip sound = passiveSounds[randomIndex];
            crashtalker.PlayOneShot(sound);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("whatisGround"))
        {
            Vector3 collisionPoint = collision.GetContact(0).point;
            GameObject jp = Instantiate(thudParticles, collisionPoint, Quaternion.identity);
            Destroy(jp, 4f);
            int c3 = Random.Range(0, thudSounds.Length);
            AudioClip c2 = thudSounds[c3];
            hitSounds.PlayOneShot(c2);
            if (Random.value < 0.10f)
            {
                int c4 = Random.Range(0, crashTalk.Length);
                AudioClip c5 = crashTalk[c4];
                crashtalker.PlayOneShot(c5);
            }
        }

        if (collision.collider.CompareTag("Player"))
        {
            Vector3 direction = transform.position - collision.transform.position;
            direction.y = 0f;


            float jumpForce = Mathf.Sqrt(2f * Physics.gravity.magnitude * 1);

            jumpForce *= 1.5f;

            rb.AddForce(direction.normalized * jumpForce, ForceMode.VelocityChange);
        }


        if (!ded)
        {
            if (collision.gameObject.CompareTag("PlayerAttack"))
            {
                takeDamage();
                if (health > 0)
                {
                    if (!isTriggered)
                    {
                       audioSource.PlayOneShot(targetedSound);
                        if (Random.value < 0.25f)
                        {
                            audioSource.PlayOneShot(letsGetItOn);
                        }
                        isTriggered = true;
                        wandering = false;
                    }
                    int randomIndex = Random.Range(0, damageSounds.Length);
                    AudioClip hitSound2 = damageSounds[randomIndex];
                    audioSource.clip = hitSound2;
                    audioSource.PlayOneShot(audioSource.clip);
                    Vector3 collisionPoint = collision.GetContact(0).point;
                    GameObject ded = Instantiate(collisionParticles, collisionPoint, Quaternion.identity);
                    Destroy(ded, 5f);
                }
                else
                {
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
                    Destroy(GetComponent<BoxCollider>());
                    Destroy(this.gameObject, 4.2f);
                    Destroy(this); // add change to ragdoll
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
                            CapsuleCollider caps = child.gameObject.GetComponent<CapsuleCollider>();
                            if (boxCollider != null)
                            {
                                boxCollider.enabled = true;
                            }
                            if(caps != null)
                            {
                                caps.enabled = false;
                            }
                        }
                    }
                }
            }
        }
    }
    private void Update()
    {
        if (!isTriggered)
        {
            // Perform raycast to detect player
            RaycastHit hit;
            Vector3 rayDirection = player.position - transform.position;

            if (Physics.Raycast(transform.position, rayDirection, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    isTriggered = true;
                }
            }
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (isShooting)
            {
                float wanderForceMagnitude = Random.Range(wanderForceMin, wanderForceMax);
                rb.AddForce(transform.up * wanderForceMagnitude);
                if (distanceToPlayer > shootingRange)
                {
                    // Player is out of shooting range, stop shooting
                    isShooting = false;
                }
                else
                {
                    // Continue shooting at the player
                    Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    ShootAtPlayer();
                    return; // Skip the remaining code in this frame
                }
            }
            else
            {
                float wanderForceMagnitude = Random.Range(wanderForceMin, wanderForceMax);
                rb.AddForce(transform.up * wanderForceMagnitude);
                if (distanceToPlayer <= shootingRange)
                {
                    // Player is in shooting range, start shooting
                    isShooting = true;
                    rb.velocity = Vector3.zero;
                    ShootAtPlayer();
                    return; // Skip the remaining code in this frame
                }
                else if (distanceToPlayer > detectionRange)
                {
                    // Player is out of range, continue chasing
                    Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    rb.velocity = (player.position - transform.position).normalized * flySpeed;
                    return; // Skip the remaining code in this frame
                }
                else
                {
                    // Fly towards the player
                    rb.velocity = (player.position - transform.position).normalized * flySpeed;

                    // Smoothly rotate towards the player
                    Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }

        if (wandering)
        {
            if (changeDirectionTimer <= 0f)
            {
                // Generate a new random movement direction
                currentDirection = Random.insideUnitSphere.normalized;
                changeDirectionTimer = Random.Range(0f, maxChangeInterval);
            }
            else
            {
                changeDirectionTimer -= Time.deltaTime;
            }

            // Calculate the random force magnitude
            float wanderForceMagnitude = Random.Range(wanderForceMin, wanderForceMax);

            // Apply the random force in the current direction
            Vector3 wanderForce = currentDirection * wanderForceMagnitude;
            rb.AddForce(wanderForce, ForceMode.Force);

            // Smoothly rotate towards the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ShootAtPlayer()
    {
        if (Time.time >= nextBurstTime)
        {
            if (!isShooting)
            {
                float randomValue = Random.Range(0f, 1f);
                if (randomValue <= 0.6f) // Adjust this value to change the shooting probability (e.g., 0.6f for 60%)
                {
                    isShooting = true;
                    nextShotTime = Time.time + Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
                }
            }

            if (isShooting && Time.time >= nextShotTime)
            {
                int randomIndex = Random.Range(0, 2);
                Vector3 bulletDirection = (player.position - transform.position).normalized;

                // Introduce inaccuracy to bullet aiming
                bulletDirection += Random.insideUnitSphere * bulletInaccuracy;

                if (randomIndex == 0)
                {
                    GameObject bullet = Instantiate(bulletPrefab, muzzle1.position, Quaternion.identity);
                    Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
                    bulletRB.velocity = bulletDirection * bulletSpeed;
                    muzzleFlare.Play();
                }
                else
                {
                    GameObject bullet = Instantiate(bulletPrefab, muzzle2.position, Quaternion.identity);
                    Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
                    bulletRB.velocity = bulletDirection * bulletSpeed;
                    muzzleFlare2.Play();
                }

                int r2 = Random.Range(0, gunShots.Length);
                AudioClip sound = gunShots[r2];
                audioSource.PlayOneShot(sound);

                nextShotTime = Time.time + Random.Range(minTimeBetweenShots, maxTimeBetweenShots);

                // Set the next burst time randomly between minTimeBetweenBursts and maxTimeBetweenBursts
                nextBurstTime = Time.time + Random.Range(minTimeBetweenBursts, maxTimeBetweenBursts);

                isShooting = false;
            }
        }
    }

    public void takeDamage()
    {
        health--;
        FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
        if(health < 1)
        {
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
            Destroy(GetComponent<BoxCollider>());
            Destroy(this.gameObject, 4.2f);
            Destroy(this); // add change to ragdoll
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
                    CapsuleCollider caps = child.gameObject.GetComponent<CapsuleCollider>();
                    if (boxCollider != null)
                    {
                        boxCollider.enabled = true;
                    }
                    if (caps != null)
                    {
                        caps.enabled = false;
                    }
                }
            }
        
    }
    }
}
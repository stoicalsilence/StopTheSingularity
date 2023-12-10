using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{
    [Header("Balance Values")]
    public int health = 7;
    public float detectionRange = 25f;
    public float minimumRange = 16f;
    public float movementSpeedWalking = 5f;
    public float movementSpeedRunning = 7.5f;
    public float shootInterval = 1f;
    public float bulletSpeed = 25f;
    public float bulletInaccuracy = 5f;
    public bool inCover = false;
    public Vector3 coverbonus;
    public int maxAmmo = 20;
    public int ammo = 20;
    public float sidestepInterval= 2f;

    [Header("Necessary References")]
    public Transform bullethole;
    public float damping = 2f;
    public GameObject bulletPrefab;
    public AudioSource audioSource;
    public GameObject m4Pickup;
    public List<Soldier> squad;
    public ParticleSystem muzzleFlare;
    public AudioClip[] gunShots;
    public GameObject magazineObject;
    public Transform magDropPos;
    public GameObject orangeLight;

    public bool triggered;
    private RaycastHit hitInfo;
    private RaycastHit losInfo;
    private float shootTimer = 0f;
    private float sidestepTimer = 0f;
    public float followTimer = 0f;
    private bool isDead;
    private Rigidbody rb;
    private Transform player;
    public Animator animator;
    private NavMeshAgent agent;

    [Header("Animations")]
    public AnimationClip idle;
    public AnimationClip walk;
    public AnimationClip handWave;
    public AnimationClip pointFinger;
    public AnimationClip reload;

    [Header("AudioClips")]
    public AudioClip magRelease;
    public AudioClip magInsert;
    public AudioClip[] enemySpotted;
    public AudioClip[] deathSounds;

    public bool blockAnims;
    public bool rotateTowardsPlayer;
    public Vector3 retreatSpot;
    private Vector3 previousPosition;
    private Vector3 sidestepPos;
    private Vector3 lastSeenPlayerPos;
    public GameObject[] safeSpots;

    public Transform target;
    int retryAmount = 8;
    bool running;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>().transform;
        animator.SetBool("Idle", true);
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeedWalking;
        animator.enabled = true;
        previousPosition = transform.position.normalized;
        agent.updateRotation = false;
        safeSpots = GameObject.FindGameObjectsWithTag("SafeSpot");
    }

    private void Update()
    {
        if (rotateTowardsPlayer)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
            targetRotation *= Quaternion.Euler(0f, 180, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
        }

        if (!isDead)
        {
            if (!triggered)
            {
                Vector3 raycastOrigin = transform.position;
                if (inCover) { raycastOrigin = transform.position + coverbonus; }

                if (Physics.Raycast(raycastOrigin, player.position - transform.position, out hitInfo, detectionRange))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        TurnOffAnimations();
                        if (!CheckIfAnyInSquadTriggered())
                        {
                            AlertSquad();
                        }
                        rotateTowardsPlayer = true;
                        triggered = true;
                    }
                }
            }

            if (triggered && !blockAnims)
            {
                Vector3 currentPosition = transform.position.normalized;
                Vector3 movementDirection = (currentPosition - previousPosition).normalized;
                Vector3 rightDir = transform.right;
                Vector3 forwardDir = transform.forward;

                float rightDot = Vector3.Dot(movementDirection, rightDir);
                float forwardDot = Vector3.Dot(movementDirection, forwardDir);

                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (agent.velocity.magnitude < 0.1f)
                {
                    animator.SetBool("Combat_Aiming", true);
                }

                if (agent.velocity.magnitude > 0.1f && (Mathf.Abs(agent.velocity.x) > 0.1f || Mathf.Abs(agent.velocity.z) > 0.1f) && !running)
                {
                    if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
                    {
                        // Moving forward or backward
                        if (forwardDot > 0)
                        {
                            // Moving forward
                            TurnOffAnimations();
                            animator.SetBool("MovingForward", true);
                        }
                        else if (forwardDot < 0)
                        {
                            // Moving backward
                            TurnOffAnimations();
                            animator.SetBool("MovingBackwards", true);
                        }
                    }
                    else
                    {
                        // Moving left or right
                        if (rightDot > 0)
                        {
                            // Moving right
                            TurnOffAnimations();
                            animator.SetBool("MovingRight", true);
                        }
                        else if (rightDot < 0)
                        {
                            // Moving left
                            TurnOffAnimations();
                            animator.SetBool("MovingLeft", true);
                        }
                    }
                }
                Physics.Raycast(transform.position, player.position - transform.position, out losInfo, detectionRange - 6);


                if (distanceToPlayer < minimumRange && losInfo.collider.CompareTag("Player"))
                {
                    followTimer = 0;
                    lastSeenPlayerPos = losInfo.point;
                    AttackPlayer();
                }
                else if(!losInfo.collider.CompareTag("Player"))
                {
                    followTimer += Time.deltaTime;
                    if (followTimer < 3)
                    {
                        AttackPlayer();
                    }
                    else if(followTimer > 6 && followTimer < 19)
                    {
                        agent.SetDestination(lastSeenPlayerPos);
                    }
                    else if(followTimer > 20)
                    {
                        rotateTowardsPlayer = false;
                        triggered = false;
                        TurnOffAnimations();
                        animator.SetBool("Idle", true);
                    }
                }
                else
                {
                    agent.ResetPath();
                }

                previousPosition = currentPosition.normalized;

            }

        }
    }

    public void FindSpotToReload()
    {
        List<GameObject> validSafeSpots = new List<GameObject>();

        foreach (GameObject safeSpot in safeSpots)
        {
            float distanceToSafeSpot = Vector3.Distance(transform.position, safeSpot.transform.position);

            if (distanceToSafeSpot <= detectionRange)
            {
                validSafeSpots.Add(safeSpot);
            }
        }
        Debug.Log(validSafeSpots.Count);
        if (validSafeSpots.Count > 0 && Random.Range(0,100) < 75)
        {
            Debug.Log("Wo");
            int randomIndex = Random.Range(0, validSafeSpots.Count);
            retreatSpot = validSafeSpots[randomIndex].transform.position;
            TurnOffAnimations();
            running = true;
            rotateTowardsPlayer = false;
            agent.updateRotation = true;
            animator.SetBool("Combat_Run", true);
        }
        else
        {
            retreatSpot = transform.position;
        }
    }

    private void AttackPlayer()
    {
        if (ammo > 0)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                muzzleFlare.Play();
                ShootBullet();
                shootTimer = Random.Range(0.05f, shootInterval);
            }
            sidestepTimer += Time.deltaTime;
            if (sidestepTimer >= sidestepInterval)
            {
                sidestepPos = SidestepCalc();
                TurnOffAnimations();
                animator.SetBool("Combat_Aiming", true);
                agent.SetDestination(sidestepPos);
                sidestepTimer = 0;
            }
            if (transform.position == sidestepPos)
            {
                sidestepPos = new Vector3();
                agent.ResetPath();
            }
        }
        else
        {
            if (!blockAnims)
            {
                FindSpotToReload();
                agent.SetDestination(retreatSpot);
                float vicinityThreshold = 1;

                if (Vector3.Distance(transform.position, retreatSpot) <= vicinityThreshold-0.9f) { Reload(); }
            }
        }
    }

    public void Reload()
    {
        blockAnims = true;
        TurnOffAnimations();
        animator.Play(reload.name);
        ammo = maxAmmo;
        Invoke("playMagRelease", 0.35f);
        Invoke("playMagInsert", 1.50f);
        Invoke("DropMagazine", 0.40f);
        Invoke("resumeAiming", reload.length);
        Invoke("removeDestination", reload.length);
    }

    public void resumeAiming()
    {
        animator.SetBool("Combat_Aiming", true);
        running = false;
        rotateTowardsPlayer = true;
        agent.updateRotation = false;
        blockAnims = false;
    }

    public void removeDestination()
    {
        agent.ResetPath();
    }

    public void stopBlockingAnims()
    {
        blockAnims = false;
    }

    public void TurnOffAnimations()
    {
        animator.SetBool("Combat_Aiming", false);
        animator.SetBool("Idle", false);
        animator.SetBool("MovingBackwards", false);
        animator.SetBool("MovingForward", false);
        animator.SetBool("MovingRight", false);
        animator.SetBool("MovingLeft", false);
        animator.SetBool("Combat_Run", false);
    }

    public void TriggerSquad()
    {
        if (squad.Count > 0)
        {
            foreach (Soldier soldier in squad)
            {
                if (soldier.triggered)
                {
                    triggered = true;
                }
            }
        }
    }

    public bool CheckIfAnyInSquadTriggered()
    {
        bool toreturn = false;
        foreach (Soldier sol in squad)
        {
            if (sol.triggered) toreturn = true;
        }
        return toreturn;
    }

    public void AlertSquad()
    {
        blockAnims = true;
        animator.Play(pointFinger.name);
        Invoke("stopBlockingAnims", pointFinger.length);
        Invoke("TriggerSquad", pointFinger.length);
        Invoke("resumeAiming", pointFinger.length);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead)
        {
            if (collision.gameObject.CompareTag("PlayerAttack"))
            {
                takeDamage();
            }
        }
    }

    public void takeDamage()
    {
        health--;
        FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
        if (health < 1)
        {
            GameObject m4 = Instantiate(m4Pickup, transform.position, Quaternion.identity);
            m4.GetComponent<AssaultRifle>().ammoInMag = ammo;
            isDead = true;
            FindObjectOfType<KillText>().getReportedTo();
            Destroy(this.gameObject, 4.2f);
        }
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * detectionRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, detectionRange, NavMesh.AllAreas);
        return hit.position;
    }

    public void DropMagazine()
    {
        GameObject mag = Instantiate(magazineObject, magDropPos.position, Quaternion.identity * Quaternion.EulerAngles(0,90,0));
        Destroy(mag, 6f);
    }

    private void ShootBullet()
    {
        StartCoroutine(bulletLight());
        ammo--;
        int randomIndex = Random.Range(0, gunShots.Length);
        AudioClip sound = gunShots[randomIndex];
        audioSource.PlayOneShot(sound);
        Vector3 directionToPlayer = player.position - bullethole.position;
        directionToPlayer = ApplyBulletInaccuracy(directionToPlayer);

        GameObject bulletObject = Instantiate(bulletPrefab, bullethole.position, Quaternion.identity);
        Destroy(bulletObject, 3f);
        Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();

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
        float horizontalInaccuracyAngle = Random.Range(-bulletInaccuracy, bulletInaccuracy);
        float verticalInaccuracyAngle = Random.Range(-bulletInaccuracy, bulletInaccuracy);

        Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalInaccuracyAngle, Vector3.up);
        direction = horizontalRotation * direction;

        Quaternion verticalRotation = Quaternion.AngleAxis(verticalInaccuracyAngle, Vector3.right);
        direction = verticalRotation * direction;

        return direction;
    }

    public Vector3 SidestepCalc()
    {
        int random = Random.Range(0, 4);

        Vector3 chosenPos = transform.position;

        float distance = Random.Range(1.0f, 4.0f);

        if (random == 0)
        {
            // Move forward
            chosenPos += transform.forward * distance;
        }
        else if (random == 1)
        {
            // Move left
            chosenPos -= transform.right * distance;
        }
        else if (random == 2)
        {
            // Move right
            chosenPos += transform.right * distance;
        }
        else if (random == 3)
        {
            // Move backward
            chosenPos -= transform.forward * distance;
        }

        return chosenPos;
    }

    void playMagRelease()
    {
        audioSource.PlayOneShot(magRelease);    
    }
    void playMagInsert()
    {
        audioSource.PlayOneShot(magInsert);
    }
}

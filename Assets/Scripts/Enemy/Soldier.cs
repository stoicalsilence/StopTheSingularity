using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{
    [Header("Type of Soldier")]
    public bool isM4Soldier;
    public bool isUziSoldier;
    public bool isShieldSoldier;
    public bool isShotgunSoldier;

    [Header("Balance Values")]
    public int health = 6;
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
    public float chanceToRetreatForReload;
    public bool isIdleWanderer;

    [Header("Necessary References")]
    public Transform bullethole;
    public float damping = 2f;
    public GameObject bulletPrefab;
    public AudioSource audioSource;
    public GameObject m4Pickup;
    public GameObject uziPickup;
    public GameObject glockPickup;
    public GameObject shotgunPickup;
    public List<Soldier> squad;
    public ParticleSystem muzzleFlare;
    public AudioClip[] gunShots;
    public GameObject magazineObject;
    public Transform magDropPos;
    public GameObject orangeLight;
    public GameObject bloodParticles;
    public List<Transform> possibleTargets = new List<Transform>();
    public List<Transform> seenTargets = new List<Transform>();

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
    public AnimationClip dieCrouching1;
    public AnimationClip meleeAttackVsStanding;
    public AnimationClip meleeAttackVsCrouching;

    public AnimationClip[] standingDeaths;

    [Header("AudioClips")]
    public AudioClip magRelease;
    public AudioClip magInsert;
    public AudioClip radioBeep;
    public AudioClip shotgunPump;
    public AudioClip[] meleeAttackSounds;
    public AudioClip[] enemySpotted;
    public AudioClip[] enemyReaquired;
    public AudioClip[] acknowledged;
    public AudioClip[] deathSounds;
    public AudioClip[] alertThroughHit;
    public AudioClip[] lostTheEnemy;
    public AudioClip[] reactToSquadDeath;
    public AudioClip[] painSounds;
    public AudioClip[] intermittendCombatSpouts;
    public AudioClip[] intermittendIdleSpouts;
    public AudioClip[] footsteps;

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
    bool runningToReload;
    public bool crouching;
    public bool meleeAttacking;
    Vector3 colliderStandingScale;
    float colliderStandingHeight;
    private float lastFootstepTime;
    private float lastCombatSpoutTime;
    public float combatSpoutInterval;
    private float lastIdleSpoutTime;
    private float lastWanderTime;
    public float idleSpoutInterval;
    public float wanderInterval;

    public bool shouldIdleChatter;
    public bool shouldCombatChatter;
    bool isPlayerAlive;

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

        colliderStandingScale = transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>().center;
        colliderStandingHeight = transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>().height;
        lastIdleSpoutTime = Random.Range(0.5f, idleSpoutInterval);
        lastCombatSpoutTime = Random.Range(0.5f, combatSpoutInterval);
        lastWanderTime = Random.Range(2, wanderInterval);
        possibleTargets.Add(FindObjectOfType<Player>().transform);

    }

    private void Update()
    {
        isPlayerAlive = !player.gameObject.GetComponent<Player>().dead;
        if (player.gameObject.GetComponent<Player>().dead && triggered)
        {
            //say enemy is history
            Untrigger();
        }
        if (isPlayerAlive)
        {
            if (rotateTowardsPlayer)
            {
                Vector3 directionToPlayer = ReturnFirstValidTarget().position - transform.position;
                directionToPlayer.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
                targetRotation *= Quaternion.Euler(0f, 180, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
            }

            if (!isDead)
            {
                float footstepInterval = 1.5f / agent.velocity.magnitude;
                float timeSinceLastFootstep = Time.time - lastFootstepTime;

                if (timeSinceLastFootstep >= footstepInterval)
                {
                    AudioClip footstepSound = footsteps[Random.Range(0, footsteps.Length)];
                    audioSource.PlayOneShot(footstepSound);
                    lastFootstepTime = Time.time;
                }

                if (!triggered)
                {
                    foreach (Transform target in possibleTargets)
                    {
                        if (target.gameObject != null)
                        {
                            Vector3 raycastOrigin = transform.position;
                            if (inCover) { raycastOrigin = transform.position + coverbonus; }

                            Vector3 playerDirection = target.position - transform.position;
                            float angleToPlayer = Vector3.Angle(transform.forward, playerDirection);
                            if (angleToPlayer <= 90f)
                            {
                                if (Physics.Raycast(raycastOrigin, playerDirection, out hitInfo, detectionRange))
                                {
                                    if (hitInfo.collider.CompareTag("Player"))
                                    {
                                        if (!CheckIfAnyInSquadTriggered())
                                        {
                                            AlertSquad();
                                            Debug.Log(hitInfo.transform);
                                            seenTargets.Add(hitInfo.transform);
                                        }
                                        GetTriggered();
                                    }

                                    if (hitInfo.collider.CompareTag("EnemyBody"))
                                    {
                                        if (!CheckIfAnyInSquadTriggered())
                                        {
                                            AlertSquad();
                                            seenTargets.Add(hitInfo.transform);
                                        }
                                        GetTriggered();
                                    }
                                }
                            }
                        }
                    }
                    if (intermittendIdleSpouts.Length > 0)
                    {
                        HandleIntermittendIdleSpouts();
                    }

                    if (isIdleWanderer)
                    {
                        if (agent.velocity.magnitude > 1)
                        {
                            animator.SetBool("Idle", false);
                            animator.SetBool("MovingForward", true);
                        }
                        else
                        {
                            animator.SetBool("MovingForward", false);
                            animator.SetBool("Idle", true);
                        }
                        lastWanderTime += Time.deltaTime;
                        if (lastWanderTime >= wanderInterval)
                        {
                            lastWanderTime = Random.Range(2, wanderInterval);
                            Vector3 dir = Calculate8Direction();
                            agent.SetDestination(dir);
                            agent.updateRotation = true;
                        }
                    }
                }

                if (triggered && !blockAnims)
                {
                    if (running)
                    {
                        if (!isShieldSoldier)
                        {
                            crouching = false;
                            animator.SetBool("Combat_Crouching", false);
                        }
                        agent.speed = movementSpeedRunning;
                        TurnOffAnimations();
                        animator.SetBool("Combat_Run", true);
                    }
                    if (!running)
                    {
                        agent.speed = movementSpeedWalking;
                        animator.SetBool("Combat_Run", false);
                    }

                    if (!isShieldSoldier)
                    {
                        if (crouching)
                        {
                            animator.SetBool("Combat_Crouching", true);
                            transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>().center = new Vector3(transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>().center.x, 0.60f, transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>().center.z);
                            transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>().height = 1.3f;
                        }
                        else
                        {
                            animator.SetBool("Combat_Crouching", false);
                            transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>().center = colliderStandingScale;
                            transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>().height = colliderStandingHeight;
                        }
                    }
                    Vector3 currentPosition = transform.position.normalized;
                    Vector3 movementDirection = (currentPosition - previousPosition).normalized;
                    Vector3 rightDir = transform.right;
                    Vector3 forwardDir = transform.forward;

                    float rightDot = Vector3.Dot(movementDirection, rightDir);
                    float forwardDot = Vector3.Dot(movementDirection, forwardDir);
                    float distanceToPlayer = 0;
                    if (ReturnFirstValidTarget().position != null)
                    {
                        distanceToPlayer = Vector3.Distance(transform.position, ReturnFirstValidTarget().position);
                    }

                    if (agent.velocity.magnitude < 0.1f && !running)
                    {
                        TurnOffAnimations();
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
                    Physics.Raycast(transform.position, ReturnFirstValidTarget().position - transform.position, out losInfo, detectionRange);
                    if (!meleeAttacking && distanceToPlayer < 2)
                    {
                        StartCoroutine(MeleeAttack());
                    }
                    if (distanceToPlayer < minimumRange && losInfo.collider != null && (losInfo.collider.CompareTag("Player") || losInfo.collider.CompareTag("EnemyBody")))
                    {
                        if (followTimer > 4)
                        {
                            int r = Random.Range(0, 100);
                            if (r < 75)
                            {
                                StartCoroutine(TransmitVoice(enemyReaquired[Random.Range(0, enemyReaquired.Length)]));
                            }
                        }
                        followTimer = 0;
                        lastSeenPlayerPos = losInfo.point;
                        if (!meleeAttacking)
                        {
                            AttackPlayer();
                        }
                        RallySquadToFight();
                    }
                    else if (losInfo.collider && losInfo.collider.gameObject.transform.root.GetComponent<Soldier>())
                    {
                        Soldier sol = losInfo.collider.gameObject.transform.root.GetComponent<Soldier>();
                        if (sol != null)
                        {
                            if (!sol.crouching && !sol.running && sol != this && sol.followTimer < 2)
                            {
                                sol.Crouch();
                            }
                        }
                    }
                    else
                    {
                        if (!runningToReload)
                            followTimer += Time.deltaTime;
                        if (followTimer < 1)
                        {
                            AttackPlayer();
                            running = false;
                            agent.updateRotation = false;
                        }
                        else if (followTimer > 3 && followTimer < 9)
                        {
                            agent.SetDestination(lastSeenPlayerPos);
                            int r = Random.Range(0, 100);
                            if (r < 25)
                            {
                                running = true;
                            }
                            if (Vector3.Distance(transform.position, lastSeenPlayerPos) <= 1f)
                            {
                                running = false;
                                TurnOffAnimations();
                                animator.SetBool("Combat_Aiming", true);
                            }
                        }
                        else if (followTimer > 7 && followTimer < 14)
                        {
                            running = false;
                            if (agent.velocity.magnitude > 1)
                            {
                                animator.SetBool("Combat_Aiming", false);
                                animator.SetBool("MovingForward", true);
                            }
                            else
                            {
                                animator.SetBool("MovingForward", false);
                                animator.SetBool("Combat_Aiming", true);
                            }
                            lastWanderTime += Time.deltaTime;
                            if (lastWanderTime >= wanderInterval)
                            {
                                lastWanderTime = Random.Range(2, wanderInterval);
                                Vector3 dir = Calculate8Direction();
                                agent.SetDestination(dir);
                                agent.updateRotation = true;
                            }
                        }
                        else if (followTimer > 15)
                        {
                            Untrigger();
                            isIdleWanderer = true;
                        }
                    }

                    previousPosition = currentPosition.normalized;

                    if (followTimer < 2 && intermittendCombatSpouts.Length > 0)
                    {
                        HandleIntermittendCombatSpouts();
                    }
                }

            }

            if (isDead)
            {
                rotateTowardsPlayer = false;
                agent.ResetPath();
            }
        }
    }

    private void HandleIntermittendCombatSpouts()
    {
        if (shouldCombatChatter && squad.Count > 0)
        {
            lastCombatSpoutTime += Time.deltaTime;
            if (lastCombatSpoutTime >= combatSpoutInterval)
            {
                StartCoroutine(TransmitVoice(intermittendCombatSpouts[Random.Range(0, intermittendCombatSpouts.Length)]));
                lastCombatSpoutTime = Random.Range(5, combatSpoutInterval);
            }
        }
    }

    private void HandleIntermittendIdleSpouts()
    {
        if (shouldIdleChatter)
        {
            lastIdleSpoutTime += Time.deltaTime;
            if (lastIdleSpoutTime >= idleSpoutInterval)
            {
                StartCoroutine(TransmitVoice(intermittendIdleSpouts[Random.Range(0, intermittendIdleSpouts.Length)]));
                lastIdleSpoutTime = Random.Range(5, idleSpoutInterval);
            }
        }
    }

    public void FindSpotToReload()
    {
        crouching = false;
        List<GameObject> validSafeSpots = new List<GameObject>();

        foreach (GameObject safeSpot in safeSpots)
        {
            float distanceToSafeSpot = Vector3.Distance(transform.position, safeSpot.transform.position);

            if (distanceToSafeSpot <= detectionRange)
            {
                validSafeSpots.Add(safeSpot);
            }
        }
        int r = Random.Range(0, 100);
        if (validSafeSpots.Count > 0 && r < chanceToRetreatForReload)
        {
            runningToReload = true;
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
            if (!crouching)
            {
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
        }
        else
        {
            if (!blockAnims && !runningToReload)
            {
                FindSpotToReload();
                running = true;
                agent.SetDestination(retreatSpot);
                if (Vector3.Distance(transform.position, retreatSpot) <= 2) { Reload(); }
            }
            if (runningToReload)
            {
                if (Vector3.Distance(transform.position, retreatSpot) <= 2)
                {
                    running = true;
                    Reload();
                }
            }
        }
    }

    public void Reload()
    {
        rotateTowardsPlayer = false;
        followTimer = 1.2f;
        runningToReload = false;
        running = false;
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
                if (!soldier.triggered)
                {
                    soldier.GetTriggered();
                    int r = Random.Range(0, 100);
                    if (r < 50)
                    {
                        soldier.StartCoroutine(soldier.TransmitVoice(soldier.acknowledged[Random.Range(0, soldier.acknowledged.Length)]));
                    }
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

    public void GetTriggered()
    {
        TurnOffAnimations();
        triggered = true;
        rotateTowardsPlayer = true;
    }

    public void AlertSquad()
    {
        blockAnims = true;
        animator.Play(pointFinger.name);
        StartCoroutine(TransmitVoice(enemySpotted[Random.Range(0, enemySpotted.Length)]));
        Invoke("stopBlockingAnims", pointFinger.length);
        Invoke("TriggerSquad", pointFinger.length);
        Invoke("resumeAiming", pointFinger.length);
    }

    public void AlertSquadByGettingShot()
    {
        blockAnims = true;
        animator.Play(pointFinger.name);
        audioSource.PlayOneShot(alertThroughHit[Random.Range(0, alertThroughHit.Length)]);
        Invoke("stopBlockingAnims", pointFinger.length);
        Invoke("TriggerSquad", pointFinger.length);
        Invoke("resumeAiming", pointFinger.length);
    }

    public void RallySquadToFight()
    {
        foreach(Soldier sol in squad)
        {
            if (sol.followTimer > 6)
            {
                sol.GetTriggered();
                sol.followTimer = 1.2f;
                sol.lastSeenPlayerPos = ReturnFirstValidTarget().position;
            }
        }
    }

    public void Crouch()
    {
        crouching = true;
        Invoke("StopCrouching", 3.9f);
    }

    void StopCrouching()
    {
        crouching = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead)
        {
            Vector3 collisionPoint = collision.GetContact(0).point;
            if (collision.gameObject.CompareTag("PlayerAttack"))
            {
                takeDamage();
                GameObject blood = Instantiate(bloodParticles, collisionPoint, Quaternion.identity);
                StartCoroutine(DisableBlood(blood, 0.39f));
                Destroy(blood, 4f);
            }
        }

        if(isDead && collision.gameObject.CompareTag("PlayerAttack"))
        {
            Vector3 collisionPoint = collision.GetContact(0).point;
            GameObject blood = Instantiate(bloodParticles, collisionPoint, Quaternion.identity);
            StartCoroutine(DisableBlood(blood, 0.39f));
            Destroy(blood, 4f);
        }

        if (collision.gameObject.CompareTag("EnemyLightAttack"))
        {
            Physics.IgnoreCollision(transform.GetChild(0).GetComponent<Collider>(), collision.collider, true);
        }
    }

    IEnumerator DisableBlood(GameObject ps, float time)
    {
        yield return new WaitForSeconds(time);
        ps.GetComponent<ParticleSystem>().Stop();
        ps.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
    }
    public void takeDamage()
    {
        bool ambush = false;
        if (!triggered)
        {
            ambush = true;
            if (!CheckIfAnyInSquadTriggered())
            {
                AlertSquadByGettingShot();
            }
            GetTriggered();
        }
            if (Random.Range(0, 100) < 25 && !ambush)
            {
                AudioClip clip = painSounds[Random.Range(0, painSounds.Length)];
                audioSource.PlayOneShot(clip);
            }
        
        health--;
        FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
        if (health < 1)
        {
            if (isM4Soldier)
            {
                GameObject m4 = Instantiate(m4Pickup, transform.position, Quaternion.identity);
                m4.GetComponent<AssaultRifle>().ammoInMag = ammo;
            }
            if (isUziSoldier)
            {
                GameObject uzi = Instantiate(uziPickup, transform.position, Quaternion.identity);
                uzi.GetComponent<Uzi>().ammoInMag = ammo;
            }
            if (isShieldSoldier)
            {
                GameObject glock = Instantiate(glockPickup, transform.position, Quaternion.identity);
                glock.GetComponent<Firearm>().ammoInMag = ammo;
            }
            if (isShotgunSoldier)
            {
                GameObject shotgun = Instantiate(shotgunPickup, transform.position, Quaternion.identity);
                shotgun.GetComponent<Shotgun>().ammoInMag = ammo;
            }

            //TurnOffAnimations();
            if (!isDead)
            {
                audioSource.Stop();
                isDead = true;
                rotateTowardsPlayer = false;
                audioSource.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]);
                if (!crouching || isShieldSoldier)
                {
                    AnimationClip clip = standingDeaths[Random.Range(0, standingDeaths.Length)];
                    animator.Play(clip.name);
                }
                else
                {
                    animator.Play(dieCrouching1.name);
                }
            }

            GameObject blood1 = Instantiate(bloodParticles, transform.position + new Vector3(0.1f, 0.5f, 0), Quaternion.identity);
            GameObject blood2 = Instantiate(bloodParticles, transform.position + new Vector3(0.1f,1.2f,0), Quaternion.identity);
            GameObject blood3 = Instantiate(bloodParticles, transform.position + new Vector3(-0.1f,1.0f,0), Quaternion.identity);
            StartCoroutine(DisableBlood(blood1, 0.39f));
            StartCoroutine(DisableBlood(blood2, 0.39f));
            StartCoroutine(DisableBlood(blood3, 0.39f));
            Destroy(blood1, 2f);
            Destroy(blood2, 2f);
            Destroy(blood3, 2f);
            FindObjectOfType<KillText>().getReportedTo();
            SquadReactToDeath();
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
        Vector3 directionToPlayer = ReturnFirstValidTarget().position - bullethole.position;
        directionToPlayer = ApplyBulletInaccuracy(directionToPlayer);

        GameObject bulletObject = Instantiate(bulletPrefab, bullethole.position, Quaternion.identity);
        Destroy(bulletObject, 3f);
        Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();

        bulletRigidbody.velocity = directionToPlayer.normalized * bulletSpeed;


        if (isShotgunSoldier)
        {
            for (int i = 0; i < 3; i++)
            {
                ammo++;
                Invoke("shotGunShot", 0.06f);
            }

            Invoke("pumpShotgunSound", 0.2f);
        }
    }

    void shotGunShot()
    {
        StartCoroutine(bulletLight());
        ammo--;
        int randomIndex = Random.Range(0, gunShots.Length);
        AudioClip sound = gunShots[randomIndex];
        audioSource.PlayOneShot(sound);
        Vector3 directionToPlayer = ReturnFirstValidTarget().position - bullethole.position;
        directionToPlayer = ApplyBulletInaccuracy(directionToPlayer);

        GameObject bulletObject = Instantiate(bulletPrefab, bullethole.position, Quaternion.identity);
        Destroy(bulletObject, 3f);
        Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();

        bulletRigidbody.velocity = directionToPlayer.normalized * bulletSpeed;
    }
    void pumpShotgunSound()
    {
        audioSource.PlayOneShot(shotgunPump);
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

    private Vector3 Calculate8Direction()
    {
        int random = Random.Range(0, 8);
        float distance = Random.Range(1f, 8.0f);

        Vector3 newPosition = transform.position;

        switch (random)
        {
            case 0:
                // Move forward
                newPosition += transform.forward * distance;
                break;
            case 1:
                // Move forward-left
                newPosition += (transform.forward - transform.right).normalized * distance;
                break;
            case 2:
                // Move left
                newPosition -= transform.right * distance;
                break;
            case 3:
                // Move back-left
                newPosition -= (transform.forward + transform.right).normalized * distance;
                break;
            case 4:
                // Move backward
                newPosition -= transform.forward * distance;
                break;
            case 5:
                // Move back-right
                newPosition -= (-transform.forward + transform.right).normalized * distance;
                break;
            case 6:
                // Move right
                newPosition += transform.right * distance;
                break;
            case 7:
                // Move forward-right
                newPosition += (transform.forward + transform.right).normalized * distance;
                break;
        }

        return newPosition;
    }

    void playMagRelease()
    {
        audioSource.PlayOneShot(magRelease);    
    }
    void playMagInsert()
    {
        audioSource.PlayOneShot(magInsert);
    }

    public IEnumerator TransmitVoice(AudioClip voiceClip)
    {
        PlayRadioBeep();
        yield return new WaitForSeconds(radioBeep.length -0.1f);
        audioSource.PlayOneShot(voiceClip);
        yield return new WaitForSeconds(voiceClip.length - 0.1f);
        PlayRadioBeep();
    }

    void PlayRadioBeep()
    {
        audioSource.PlayOneShot(radioBeep);
    }

    public IEnumerator MeleeAttack()
    {
        if (!meleeAttacking)
        {
            meleeAttacking = true;
            AnimationClip meleeAttackAnim = meleeAttackVsStanding;
            if (player.gameObject.GetComponent<PlayerMovement>().isCrouching)
            {
                meleeAttackAnim = meleeAttackVsCrouching;
            }
            TurnOffAnimations();
            blockAnims = true;
            animator.Play(meleeAttackAnim.name);
            audioSource.PlayOneShot(meleeAttackSounds[Random.Range(0, meleeAttackSounds.Length)]);
            yield return new WaitForSeconds(meleeAttackAnim.length / 2);
            player.gameObject.GetComponent<Player>().takeDamage(5);
            Vector3 awayDirection = (ReturnFirstValidTarget().transform.position - transform.position).normalized;
            player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            player.GetComponent<Rigidbody>().AddForce(awayDirection * 5, ForceMode.Impulse);
            yield return new WaitForSeconds(meleeAttackAnim.length / 2);
            resumeAiming();
            meleeAttacking = false;
        }
    }

    public void Untrigger()
    {
        rotateTowardsPlayer = false;
        triggered = false;
        TurnOffAnimations();
        animator.SetBool("Idle", true);
        if(Random.Range(0,100) < 25)
        StartCoroutine(TransmitVoice(lostTheEnemy[Random.Range(0, lostTheEnemy.Length)]));
    }

    public void SquadReactToDeath()
    {
        if(squad.Count > 0)
        {
            foreach(Soldier sol in squad)
            {
                if(sol && (Random.Range(0,100) < 5) && !sol.isDead)
                {
                    sol.StartCoroutine(sol.TransmitVoice(reactToSquadDeath[Random.Range(0, reactToSquadDeath.Length)]));
                    return;
                }
            }
        }
    }

    public Transform ReturnFirstValidTarget()
    {
        foreach(Transform g in possibleTargets)
        {
            if(g.gameObject != null)
            {
                return g;
            }
            if(g.gameObject == null)
            {
                possibleTargets.Remove(g);
            }
        }
        Untrigger();
        return null;
    }
}

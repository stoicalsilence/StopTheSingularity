using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Getty : MonoBehaviour
{
    public NavMeshAgent agent;
    public KjoyunBodyPart Legs;
    public KjoyunBodyPart LeftArm;
    public KjoyunBodyPart RightArm;
    public KjoyunBodyPart Body;

    public GameObject legDestroyedParticles;
    public GameObject leftArmDestroyedParticles;
    public GameObject rightArmDestroyedParticles;
    public GameObject bodyDestroyedParticles;

    public GameObject leftArmModelToDestroy;
    public GameObject rightArmModelToDestroy;
    bool lArmDead;
    bool RArmDead;

    public Transform player;
    public Transform gunTip;
    public Transform ejectionPoint;
    public Transform missilePoint;

    public Animator animator;

    bool ischasing, attacking, rotateTowardsPlayer;

    public AudioSource gunAudioSource;
    public AudioSource missileLauncherAudioSource;

    public GameObject bullet, orangeLight, bulletCasing;
    public GameObject missile;
    public ParticleSystem muzzleFlare, muzzleSmoke, missileShotParticles, missileShotSmoke;

    public float bulletInaccuracy;
    bool dead = false;
    public bool triggered;
    public GameObject slidersContainer;
    public AudioSource generalAudiosource;

    bool canttogglechasing;

    public AnimationClip Aim, Shot, Unaim, missileshot, init;

    public AudioClip gunshot1, gunshot2, mechmove, mechmove2, thump;

    public GameObject tankerPrefab;

    public AudioSource BGM;

    public AudioClip drill;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>().transform;
       // Invoke("GetTriggered", 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateTowardsPlayer)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
            targetRotation *= Quaternion.Euler(0f, 180, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);
        }

        if (agent.velocity.magnitude > 0.5f && !dead)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
        if (ischasing && !attacking && !dead)
        {
            agent.SetDestination(player.position);
        }

        if (Legs.currentHP < 1)
        {
            legDestroyedParticles.gameObject.SetActive(true);
            ischasing = false;
            agent.Stop();
            agent.ResetPath();
        }
        if (LeftArm.currentHP < 1 && !lArmDead)
        {
            lArmDead = true;
            leftArmDestroyedParticles.gameObject.SetActive(true);
            leftArmModelToDestroy.gameObject.SetActive(false);
            if (attacking)
            {
                StopCoroutine(RangedAttack());
            }
            canttogglechasing = true;
        }
        if (RightArm.currentHP < 1 && !RArmDead)
        {
            RArmDead = true;
            rightArmDestroyedParticles.gameObject.SetActive(true);
            rightArmModelToDestroy.gameObject.SetActive(false);
            if (attacking)
            {
                StopCoroutine(MissileAttack());
            }
        }

        if (Body.currentHP < 1 && !dead)
        {
            bodyDestroyedParticles.gameObject.SetActive(true);
            //FindObjectOfType<AfterPuteyBossCutscene>().StartCutscene();
            dead = true;
            FindObjectOfType<FinalBossAssorter>().hideRamp = true;
            Invoke("playdrill", 3.1f);
            animator.Play("Defeat");
            generalAudiosource.PlayOneShot(thump);
            Destroy(this.gameObject, 19);
            Invoke("TurnOffSliders", 1);
            //Invoke("playweep", 1.3f);
            FindObjectOfType<KillText>().getReportedTo();
            Invoke("nextPhase", 8);
        }
    }
    void nextPhase()
    {
        Instantiate(tankerPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    public void GetTriggered()
    {
        triggered = true;
        Invoke("playdrill", 0.25f);
        animator.Play(init.name);
        Invoke("performAttack", 5);
        Invoke("toggleIsChasing", 2);
        slidersContainer.gameObject.SetActive(true);
        BGM.Play();
        //generalAudiosource.PlayOneShot(glitchyRobotScream);
    }
    void TurnOffSliders()
    {
        slidersContainer.gameObject.SetActive(false);
        //generalAudiosource.PlayOneShot(dropCrash);
        ScreenShake.Shake(1.3f, 0.8f);
        //BGM.Stop();
    }
    void toggleIsChasing()
    {
        if (!dead)
        {
            if (!canttogglechasing)
            {
                if (Legs.currentHP < 1)
                {
                    ischasing = false;
                    return;
                }
                if (!attacking)
                {
                    if (ischasing)
                    {
                        agent.ResetPath();
                        rotateTowardsPlayer = false;
                    }
                    else
                    {
                        rotateTowardsPlayer = true;
                    }
                    ischasing = !ischasing;
                }
            }
            else
            {
                ischasing = true;
            }
            int random = Random.Range(2, 7);
            Invoke("toggleIsChasing", random);
        }
    }

    public IEnumerator MissileAttack()
    {
        if (RightArm.currentHP > 0)
        {
            attacking = true;
            bool rmove = Random.Range(0, 100) < 50;
            AudioClip move = rmove ? mechmove : mechmove2;
            generalAudiosource.PlayOneShot(move);
            animator.Play(missileshot.name);
            yield return new WaitForSeconds(1);
            missileShotSmoke.Play();
            missileShotParticles.Play();
            GameObject mis = Instantiate(missile, missilePoint.position, Quaternion.identity);
            mis.GetComponent<MissileController>().target = player;
            mis.GetComponent<MissileController>().shouldHurtPlayer = true;
            yield return new WaitForSeconds(0.4f);
            attacking = false;
        }
    }

    public IEnumerator RangedAttack()
    {
        attacking = true;
        rotateTowardsPlayer = true;

        animator.Play(Aim.name);
        bool rmove = Random.Range(0, 100) < 50;
        AudioClip move = rmove ? mechmove : mechmove2;
        gunAudioSource.PlayOneShot(move);
        int shotsFired = 0;
        yield return new WaitForSeconds(Aim.length);
        while (shotsFired < 12)
        {
            if (dead)
            {
                rotateTowardsPlayer = false;
                yield break;
            }
            bool r = Random.Range(0, 100) < 50;
            AudioClip gunshot = r ? gunshot1 : gunshot2;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            gunAudioSource.PlayOneShot(gunshot);
            
                animator.Play(Shot.name);
            
            
            if (LeftArm.currentHP < 1)
            {
                animator.Play(Unaim.name);
                yield return new WaitForSeconds(Unaim.length);
                attacking = false;
                rotateTowardsPlayer = false;
                yield break;
            }

            shotsFired++;
            muzzleFlare.Play();
            StartCoroutine(bulletLight());
            Vector3 directionToPlayer = player.position - gunTip.position;
            directionToPlayer = ApplyBulletInaccuracy(directionToPlayer);
            GameObject bulletObject = Instantiate(bullet, gunTip.position, Quaternion.identity);
            bulletObject.GetComponent<KjoyunExplosiveBullet>().isGetty = true;
            Destroy(bulletObject, 3f);
            Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = directionToPlayer.normalized * 19;

            GameObject casing = Instantiate(bulletCasing, ejectionPoint.position, Quaternion.identity);
            casing.transform.eulerAngles = new Vector3(
                    casing.transform.eulerAngles.x + 90,
                    casing.transform.eulerAngles.y,
                    casing.transform.eulerAngles.z + 50);

            float forceMagnitude = 5f;
            float torqueMagnitude = 2f;
            casing.GetComponent<Rigidbody>().AddForce(-transform.right * forceMagnitude, ForceMode.Impulse);
            casing.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * torqueMagnitude, ForceMode.Impulse);

            yield return new WaitForSeconds(Shot.length);
        }
        animator.Play(Unaim.name);
        muzzleSmoke.Play();
        yield return new WaitForSeconds(Unaim.length);
        attacking = false;
        rotateTowardsPlayer = false;
    //    canttogglechasing = true;
    //    Invoke("toggleIsChasingImmunity", 5f);
    //    ischasing = true;
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
    public IEnumerator bulletLight()
    {
        orangeLight.SetActive(true);
        yield return new WaitForSeconds(0.16f);
        orangeLight.SetActive(false);
    }

    void performAttack()
    {
        agent.ResetPath();
        if (!dead)
        {
            float playerYPos = player.transform.position.y;
            int r = Random.Range(0, 100);
            //if(r < 20 && !RArmDead)
            //{
            //    ischasing = false;
            //    StartCoroutine(MissileAttack());
            //    int random2 = Random.Range(2, 7);
            //    Invoke("performAttack", random2);
            //    return;
            //}
            if (!attacking && r < 50) //adjust this according to the map
            {
                if (RightArm.currentHP > 0)
                {
                    ischasing = false;
                    StartCoroutine(MissileAttack());
                }
                if (RightArm.currentHP < 1)
                {
                    ischasing = false;
                    StartCoroutine(RangedAttack());
                }
            }
            else if (!attacking)
            {
                if (LeftArm.currentHP > 0)
                {
                    ischasing = false;
                    StartCoroutine(RangedAttack());
                }
                if (LeftArm.currentHP < 1)
                {
                    ischasing = false;
                    StartCoroutine(MissileAttack());
                }
            }
            int random = Random.Range(2, 7);
            Invoke("performAttack", random);
        }
    }

    void playdrill()
    {
        generalAudiosource.PlayOneShot(drill);
    }
}

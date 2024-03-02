using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kyojun : MonoBehaviour
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
    public Transform player;
    public Transform gunTip;
    public Transform ejectionPoint;

    public Animator animator;
    public AnimationClip Aim, Shot, ShotHigh, Unaim, ClubAttack;
    bool ischasing, attacking, rotateTowardsPlayer;
    public AudioSource gunAudioSource;
    public AudioSource clubAudioSource;
    public AudioClip gunshot1, gunshot2, clubSmash, mechmove, mechmove2;
    public GameObject bullet, orangeLight, bulletCasing;
    public ParticleSystem muzzleFlare, muzzleSmoke;
    public ParticleSystem clubParticles1, clubParticles2, clubParticles3;
    public float bulletInaccuracy;
    bool dead = false;
    public GameObject slidersContainer;
    public AudioSource generalAudiosource;
    public AudioClip dropCrash, thump, glitchyRobotScream;
    public AudioSource BGM;
    bool canttogglechasing;
    public AudioClip defeatWeep;

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
        while (shotsFired < 10)
        {
            if (dead)
            {
                rotateTowardsPlayer = false;
                yield break;
            }
            bool r = Random.Range(0, 100) <50;
            AudioClip gunshot = r ? gunshot1 : gunshot2;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            gunAudioSource.PlayOneShot(gunshot);
            if (distanceToPlayer < 26.5f)
            {
                animator.Play(Shot.name);
            }
            else
            {
                animator.Play(ShotHigh.name);
            }
            if(LeftArm.currentHP < 1)
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
            Destroy(bulletObject, 3f);
            Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = directionToPlayer.normalized * 19;

            GameObject casing = Instantiate(bulletCasing, ejectionPoint.position, Quaternion.identity);
            casing.transform.eulerAngles = new Vector3(
                    casing.transform.eulerAngles.x + 90,
                    casing.transform.eulerAngles.y,
                    casing.transform.eulerAngles.z);

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
        canttogglechasing = true;
        Invoke("toggleIsChasingImmunity", 5f);
        ischasing = true;
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

    public IEnumerator MeleeAttack()
    {
        bool rmove = Random.Range(0, 100) < 50;
        AudioClip move = rmove ? mechmove : mechmove2;
        clubAudioSource.PlayOneShot(move);
        attacking = true;
        rotateTowardsPlayer = true;
        animator.Play(ClubAttack.name);
        yield return new WaitForSeconds(1);
        clubParticles1.Play();
        clubParticles2.Play();
        clubParticles3.Play();
        clubAudioSource.PlayOneShot(clubSmash);
        float distanceToPlayer = Vector3.Distance(clubAudioSource.gameObject.transform.position, player.position);
        if (distanceToPlayer < 30)
        {
            ScreenShake.Shake(1.3f, 0.8f);
        }
        else if(distanceToPlayer < 40 && distanceToPlayer > 30)
        {
            ScreenShake.Shake(0.7f, 0.4f);
        }
        else
        {
            ScreenShake.SmoothShake(0.5f, 0.3f);
        }
        Collider[] colliders = Physics.OverlapSphere(clubAudioSource.gameObject.transform.position, 10);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Player player = collider.GetComponent<Player>();
                if (player != null)
                {
                    Vector3 pushDirection = player.transform.position- this.clubAudioSource.gameObject.transform.position;
                    pushDirection.y = Random.Range(3, 4);

                    pushDirection = pushDirection.normalized;

                    FindObjectOfType<Player>().gameObject.GetComponent<Rigidbody>().AddForce(pushDirection * 50, ForceMode.Impulse);
                    player.takeDamage(10);
                }
            }
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(50, clubAudioSource.gameObject.transform.position, 10);
            }
            if (collider.CompareTag("Pillar"))
            {
                rb.gameObject.GetComponent<KyojunPillar>().getBroken();
            }
        }

        rotateTowardsPlayer = false;
        yield return new WaitForSeconds(1.1f);
        attacking = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>().transform;
        agent.updateRotation = true;
        slidersContainer.gameObject.SetActive(false);
        //Invoke("GetTriggered", 5);
    }

    public void GetTriggered()
    {
        Invoke("performAttack", 5);
        Invoke("toggleIsChasing", 2);
        slidersContainer.gameObject.SetActive(true);
        BGM.Play();
        //generalAudiosource.PlayOneShot(glitchyRobotScream);
    }
    // Update is called once per frame
    void Update()
    {
        if (attacking || dead)
        {
            agent.Stop();
            agent.ResetPath();
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
       
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    StartCoroutine(RangedAttack());
        //}
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    StartCoroutine(MeleeAttack());
        //}
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

        if(Legs.currentHP < 1)
        {
            legDestroyedParticles.gameObject.SetActive(true);
            ischasing = false;
            agent.Stop();
            agent.ResetPath();
        }
        if (LeftArm.currentHP < 1)
        {
            leftArmDestroyedParticles.gameObject.SetActive(true);
            leftArmModelToDestroy.gameObject.SetActive(false);
            if (attacking)
            {
                StopCoroutine(RangedAttack());
            }
            canttogglechasing = true;
        }
        if (RightArm.currentHP < 1)
        {
            rightArmDestroyedParticles.gameObject.SetActive(true);
            rightArmModelToDestroy.gameObject.SetActive(false);
            if (attacking)
            {
                StopCoroutine(MeleeAttack());
            }
        }
        if(Body.currentHP < 1)
        {
            bodyDestroyedParticles.gameObject.SetActive(true);
        }

        if(Body.currentHP < 1 && !dead)
        {
            dead = true;
            animator.Play("Defeat");
            generalAudiosource.PlayOneShot(thump);
            Destroy(this.gameObject, 19);
            Invoke("TurnOffSliders", 1);
            //Invoke("playweep", 1.3f);
            FindObjectOfType<KillText>().getReportedTo();
        }
    }

    void performAttack()
    {
        agent.ResetPath();
        if (!dead)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (!attacking && distanceToPlayer < 25)
            {
                if (RightArm.currentHP > 0)
                {
                    ischasing = false;
                    StartCoroutine(MeleeAttack());
                }
            }
            else if (!attacking)
            {
                if (LeftArm.currentHP > 0)
                {
                    ischasing = false;
                    StartCoroutine(RangedAttack());
                }
            }
            int random = Random.Range(2, 7);
            Invoke("performAttack", random);
        }
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

    void TurnOffSliders()
    {
        slidersContainer.gameObject.SetActive(false);
        generalAudiosource.PlayOneShot(dropCrash);
        ScreenShake.Shake(1.3f, 0.8f);
        BGM.Stop();
    }
    void toggleIsChasingImmunity()
    {
        canttogglechasing = false;
    }
    void playweep()
    {
        generalAudiosource.PlayOneShot(defeatWeep);
    }
}

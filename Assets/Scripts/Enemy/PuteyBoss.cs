using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PuteyBoss : MonoBehaviour
{
    public bool triggered;
    public float speed;
    public Transform player;
    public NavMeshAgent agent;
    public Rigidbody rb;
    public float damping = 2;
    private float lastFootstepTime;
    public bool playerVeryClose;
    public bool stunned;
    public bool attacking;
    public GameObject seekingMissile, missileShotParticles, stunParticles, RedFace, BlueFace, footstepParticles, bulletPrefab, orangeLight, defeatExplosion, defeatExplosion2;
    public AudioSource audioSource;
    public AudioClip missileShot, stun1, stun2, electricity, stomp, attackTelegraph, minigunShot, hurt, defeated, block;
    public Animator animator;
    public Transform footstepFXPos, minigunMuzzle, explosionSpawnPos;
    public float bulletSpeed, bulletInaccuracy;
    public ParticleSystem muzzleFlare, muzzleSmoke;
    public bool shootingMinigun;
    public Slider hpSlider;
    public int maxHP, HP;
    public float shootInterval;
    bool invincible;
    private float shootTimer = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //InvokeRepeating("PlayPassiveSounds", 0.1f, 5);
        player = FindObjectOfType<Player>().transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.updateRotation = false;
        InvokeRepeating("attackPlayer", 5, 8);
        audioSource = GetComponent<AudioSource>();
        hpSlider.maxValue = maxHP;
        hpSlider.gameObject.SetActive(false);
        RedFace.gameObject.SetActive(false);
    }
   
    void Update()
    {
        if (triggered)
        {
            if (!playerVeryClose && !stunned && !attacking)
            {
                RunAndTurnTowardsPlayer();
            }

            if (shootingMinigun)
            {
                shootTimer += Time.deltaTime;
                if (shootTimer >= shootInterval)
                {
                    muzzleFlare.Play();
                    ShootBullet();
                    shootTimer = 0.0f;
                }
            }
        }
    }

    private void RunAndTurnTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > 4f)
        {
            animator.SetBool("Walking", true);
            animator.SetBool("Idle", false);
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);

            agent.SetDestination(player.position);

            float footstepInterval = 1;
            float timeSinceLastFootstep = Time.time - lastFootstepTime;

            if (timeSinceLastFootstep >= footstepInterval)
            {
                audioSource.PlayOneShot(stomp);
                GameObject step = Instantiate(footstepParticles, footstepFXPos.transform.position, Quaternion.identity);
                step.transform.localScale = new Vector3(3, 3, 3);
                Destroy(step, 3f);
                lastFootstepTime = Time.time;
            }
        }
        else
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Idle", true);
            if (!playerVeryClose) { playerVeryClose = true; Invoke("resetPlayerVeryClose", 1); }
        }
    }

    public void startBossFight()
    {
        animator.Play("StartAnimation");
        Invoke("activateRedFace", 1.40f);
        Invoke("triggeredtrue", 3);
        hpSlider.gameObject.SetActive(true);
    }

    void triggeredtrue()
    {
        triggered = true;
    }

    void resetPlayerVeryClose()
    {
        playerVeryClose = false;
    }

    void attackPlayer()
    {
        if (!triggered || stunned || playerVeryClose) return;

        agent.enabled = false;
        attacking = true;
        animator.SetBool("Walking", false);
        animator.SetBool("Idle", false);

        int random = Random.Range(0, 100);

        if (random < 50)
        {
            animator.Play("ShootMissile");
            audioSource.PlayOneShot(attackTelegraph);
            Invoke("spawnMissile", 0.35f);
            Invoke("stopAttacking", 1.50f);
        }
        else
        {
            attacking = false;
            agent.enabled = true;
            agent.speed = 0;
            animator.Play("ShootMinigun");
            audioSource.PlayOneShot(attackTelegraph);
            Invoke("startShootingMinigun", 0.30f);
            Invoke("stopShootingMinigun", 2.20f);
            Invoke("stopAttacking", 3.50f);
        }
    }

    void startShootingMinigun()
    {
        shootingMinigun = true;
    }
    void stopShootingMinigun()
    {
        shootingMinigun = false;
        muzzleSmoke.Play();
    }
    void stopAttacking()
    {
        animator.SetBool("Walking", true);
        attacking = false;
        agent.enabled = true;
        agent.speed = speed;
    }
    void cancelInvincibility()
    {
        invincible = false;
    }
    void spawnMissile()
    {
        GameObject missile = Instantiate(seekingMissile, missileShotParticles.transform.position, Quaternion.identity);
        missile.GetComponent<MissileController>().target = player;
        missile.GetComponent<MissileController>().selfexplodetimer = 15;
        missile.GetComponent<MissileController>().shouldHurtPlayer = true;

        GameObject shotParticles = Instantiate(missileShotParticles, missileShotParticles.transform.position, Quaternion.identity);
        Destroy(shotParticles, 3f);

        audioSource.PlayOneShot(missileShot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<MissileController>())
        {
            other.gameObject.GetComponent<MissileController>().shouldHurtPlayer = false;
            other.gameObject.GetComponent<MissileController>().explode();

            animator.SetBool("Stunned", true);
            animator.SetBool("Walking", false);
            animator.SetBool("Idle", false);
            this.gameObject.tag = "Untagged";
            
            hpSlider.fillRect.GetComponent<Image>().color = Color.yellow;
            audioSource.PlayOneShot(stun1);
            audioSource.PlayOneShot(stun2);
            audioSource.PlayOneShot(electricity);
            stunned = true;
            stunParticles.SetActive(true);
            RedFace.SetActive(false);
            BlueFace.SetActive(true);
            Invoke("removeStun", 5);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerAttack")
        {
            if (stunned && HP > 0 && !invincible)
            {
                HP--;
                invincible = true;
                Invoke("cancelInvincibility", 0.1f);
                hpSlider.value = HP;
                audioSource.PlayOneShot(hurt);
            }
            else if ( stunned && HP <= 0)
            {
                HP--;
                hpSlider.value = HP;
                audioSource.Stop();
                audioSource.PlayOneShot(defeated);
                hpSlider.gameObject.SetActive(false);
                triggered = false;
                animator.Play("Defeated");
                RedFace.SetActive(false);
                BlueFace.SetActive(true);
                Destroy(stunParticles);
                Invoke("explode", 1.90f);
            }
            else if (!stunned)
            {
                audioSource.PlayOneShot(block);
            }
        }
    }

    void explode()
    {
        Instantiate(defeatExplosion, explosionSpawnPos.position, Quaternion.identity);
        Instantiate(defeatExplosion2, explosionSpawnPos.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    void removeStun()
    {
        stunned = false;
        stunParticles.SetActive(false);
        RedFace.SetActive(true);
        BlueFace.SetActive(false);
        animator.SetBool("Stunned", false);
        animator.SetBool("Walking", true);
        this.gameObject.tag = "EnemyBody";
        hpSlider.fillRect.GetComponent<Image>().color = Color.red;
    }

    void activateRedFace()
    {
        RedFace.SetActive(true);
    }
    private void ShootBullet()
    {
        muzzleFlare.Play();
        StartCoroutine(bulletLight());
        audioSource.PlayOneShot(minigunShot);
        Vector3 directionToPlayer = player.position - minigunMuzzle.position;
        directionToPlayer = ApplyBulletInaccuracy(directionToPlayer);

        GameObject bulletObject = Instantiate(bulletPrefab, minigunMuzzle.position, Quaternion.identity);
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
}

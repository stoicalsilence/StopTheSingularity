using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public GameObject seekingMissile, missileShotParticles, stunParticles, RedFace, BlueFace, footstepParticles;
    public AudioSource audioSource;
    public AudioClip missileShot, stun1, stun2, stomp;
    public Animator animator;
    public Transform footstepFXPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //InvokeRepeating("PlayPassiveSounds", 0.1f, 5);
        player = FindObjectOfType<Player>().transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.updateRotation = false;
        InvokeRepeating("attackPlayer", 5, 10);
        audioSource = GetComponent<AudioSource>();
    }
   
    void Update()
    {
        if (triggered)
        {
            if (!playerVeryClose && !stunned && !attacking)
            {
                RunAndTurnTowardsPlayer();
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
            Invoke("spawnMissile", 0.35f);
            Invoke("stopAttacking", 1.50f);
        }
        else
        {//replace with minigun attack
            animator.Play("ShootMissile");
            Invoke("spawnMissile", 0.35f);
            Invoke("stopAttacking", 1.50f);
        }
    }

    void stopAttacking()
    {
        animator.SetBool("Walking", true);
        attacking = false;
        agent.enabled = true;
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

            audioSource.PlayOneShot(stun1);
            audioSource.PlayOneShot(stun2);
            stunned = true;
            stunParticles.SetActive(true);
            RedFace.SetActive(false);
            BlueFace.SetActive(true);
            Invoke("removeStun", 5);
        }
    }

    void removeStun()
    {
        stunned = false;
        stunParticles.SetActive(false);
        RedFace.SetActive(true);
        BlueFace.SetActive(false);
        animator.SetBool("Stunned", false);
        animator.SetBool("Walking", true);
    }
}

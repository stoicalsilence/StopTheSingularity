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

    [Header("Necessary References")]
    public Transform bullethole;
    public float damping = 2f;
    public GameObject bulletPrefab;
    public AudioSource audioSource;
    public GameObject m4Pickup;
    public List<Soldier> squad;

    public AudioClip[] gunShots;

    public bool triggered;
    private RaycastHit hitInfo;
    private float shootTimer = 0f;
    private bool isDead;
    private Rigidbody rb;
    private Transform player;
    public Animator animator;
    private NavMeshAgent agent;

    [Header("Animations")]
    public AnimationClip idle;
    public AnimationClip walk;
    public AnimationClip die1;
    public AnimationClip die2;
    public AnimationClip handWave;
    public AnimationClip pointFinger;
    public AnimationClip reload;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>().transform;
        animator.SetBool("Idle", true);
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeedWalking;
        animator.enabled = true;
        animator.Play(idle.name);

        if(squad.Count > 0)
        InvokeRepeating("CheckSquadTriggered", 5, 2);
    }

    private void Update()
    {
        if (!isDead)
        {
            if (player == null)
            {
                animator.SetBool("Attacking", false);
                animator.SetBool("Idle", true);
                animator.Play(idle.name);
                return;
            }
            if (!triggered)
            {
                Vector3 coverbonus = new Vector3(0, 0.5f, 0);
                if (inCover) coverbonus = new Vector3(0, 1, 0f);
                if (Physics.Raycast(transform.position + coverbonus, player.position - transform.position, out hitInfo, detectionRange))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        triggered = true;
                        animator.SetBool("Idle", false);
                        animator.SetBool("Attacking", true);
                    }
                }
            }
            else
            {
                Vector3 directionToPlayer = player.position - transform.position;
                directionToPlayer.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
                targetRotation *= Quaternion.Euler(0f, 0, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);

                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (agent.enabled)
                {
                    agent.SetDestination(player.position);
                }

                if (distanceToPlayer < minimumRange)
                {
                    if (shootTimer >= shootInterval)
                    {
                        //muzzleFlare.Play();
                        ShootBullet();
                        shootTimer = 0.0f;
                    }
                }

            }
            }
        
    }

    public void CheckSquadTriggered()
    {
        foreach(Soldier soldier in squad)
        {
            if (soldier.triggered)
            {
                triggered = true;
            }
        }
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
            Instantiate(m4Pickup, transform.position, Quaternion.identity);
            isDead = true;
            FindObjectOfType<KillText>().getReportedTo();
            Destroy(this.gameObject, 4.2f);
            float random = Random.Range(0, 100);
            AnimationClip chosenAnim;
            if(random < 49)
            {
                chosenAnim = die1;
            }
            else
            {
                chosenAnim = die2;
            }

            //animator.Play(chosenAnim.name);
        }
    }

    private void ShootBullet()
    {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbot : MonoBehaviour
{
    public Transform player, bullethole;
    public float damping = 2f;
    public float yRotationOffset = 120f;
    public bool triggered;
    public float detectionRange = 10f;
    public float minimumRange = 3f;
    public float movementSpeed = 5f; // New speed variable
    public Animator animator;
    public Animator bobAnimation; // Reference to the animation object

    public ParticleSystem muzzleFlare;
    public GameObject orangeLight, bulletPrefab;

    public float shootInterval = 1.0f;
    public float bulletSpeed = 10.0f;
    public float bulletInaccuracy = 5.0f;

    private float shootTimer = 0.0f;

    public AudioSource audioSource;
    public AudioClip[] gunShots;

    private RaycastHit hitInfo;

    private void Start()
    {
        player = FindObjectOfType<Player>().transform;
        animator = GetComponent<Animator>(); // Assign the animator to the animation object
        animator.SetBool("Idle", true);
        bobAnimation = gameObject.transform.parent.gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (!triggered)
        {
            if (Physics.Raycast(transform.position, player.position - transform.position, out hitInfo, detectionRange))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                    triggered = true;
                    animator.SetBool("Idle", false);
                    animator.SetBool("Attacking", true);
                    bobAnimation.SetBool("Triggered", true);
                }
            }
        }
        else
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
            targetRotation *= Quaternion.Euler(0f, yRotationOffset, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < minimumRange)
            {
                animator.SetBool("AttackingStanding", true);
                bobAnimation.SetBool("Triggered", false);
                shootTimer += Time.deltaTime;
                if (shootTimer >= shootInterval)
                {
                    muzzleFlare.Play();
                    ShootBullet();
                    shootTimer = 0.0f; // Reset the timer
                }
                // Stop moving towards the player
                // Add any additional behavior you want when the player is within the minimum range
            }
            else
            {
                bobAnimation.SetBool("Triggered", true);
                animator.SetBool("AttackingStanding", false);
                animator.SetBool("Attacking", true);
                // Move towards the player with a specific speed
                transform.position += directionToPlayer.normalized * movementSpeed * Time.deltaTime;
            }
        }
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
        // Calculate a random inaccuracy angle
        float inaccuracyAngle = Random.Range(0.0f, bulletInaccuracy);
        // Rotate the direction vector around the up axis by the inaccuracy angle
        Quaternion rotation = Quaternion.AngleAxis(inaccuracyAngle, Vector3.up);
        return rotation * direction;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tanker : MonoBehaviour
{
    public GameObject explosiveBullet;
    public float launchForce = 10f;
    public int numberOfGrenades = 5;
    public AudioSource audioSource;
    public AudioClip floop, thump, mGunShot, reload;
    public Transform player;

    public Transform grenadeLauncherSpot, cannonShotSpot, mGun1hole, mGun2hole;
    bool rotateTowardsPlayer, isDriving, ded;
    public GameObject mGun1, mGun2, mGunBullet;

    public ParticleSystem exhaust;
    public AudioSource driveSource;
    public AudioSource turnSource;
    public Rigidbody rb;
    public Transform visual;
    public GameObject hitSparkles, destroyParticles;
    public AudioClip hitSound;

    public Animator animator;

    public Slider hpSlider;

    public int maxHP, HP;
    public GameObject lordPuteyPrefab;
    public AudioClip drill;
    private void Start()
    {
        hpSlider = FindObjectOfType<FinalBossAssorter>().tankerSlider;
        hpSlider.maxValue = maxHP;
        hpSlider.value = HP;
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<Player>().transform;
        Invoke("GetTriggered", 2);

        Invoke("playdrill", 0.4f);
    }
    void Update()
      {
    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        StartCoroutine("LaunchGrenades");
    //    }
    //    if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        StartCoroutine("ShootCannon");
    //    }
    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        StartCoroutine("ShootMGuns");
    //    }

        if (!ded)
        {
            Rotate();
            Movement();
        }
        if (ded)
        {
            turnSource.Stop();
            driveSource.Stop();
        }
    }

    IEnumerator LaunchGrenades()
    {
        for (int i = 0; i < numberOfGrenades; i++)
        {
            if (!ded)
            {
                audioSource.PlayOneShot(floop);
                GameObject grenade = Instantiate(explosiveBullet, grenadeLauncherSpot.position, Quaternion.identity);
                Rigidbody rb = grenade.GetComponent<Rigidbody>();
                rb.useGravity = true;

                Vector3 directionToPlayer = (player.position - grenadeLauncherSpot.position).normalized;
                float distanceToPlayer = Vector3.Distance(grenadeLauncherSpot.position, player.position);

                // Adjust the launch force based on the distance to the player
                launchForce = Mathf.Clamp(distanceToPlayer / 10, 0.5f, 2f); // This line adjusts the launch force

                // Calculate the initial velocity to reach the player
                float initialVelocity = Mathf.Sqrt(launchForce * distanceToPlayer * Physics.gravity.magnitude / Mathf.Sin(2 * 45 * Mathf.Deg2Rad));

                // Apply the initial velocity with a slight upward angle
                Vector3 launchVelocity = directionToPlayer * initialVelocity * 0.5f + Vector3.up * initialVelocity * 0.5f;

                // Apply some randomness to the launch velocity
                launchVelocity += Random.insideUnitSphere * 2f;

                rb.velocity = launchVelocity;

                float r = Random.Range(0.5f, 1.2f);
                yield return new WaitForSeconds(r);
            }
        }
        audioSource.PlayOneShot(reload);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAttack")
        {
            HP--;
            hpSlider.value = HP;
            Vector3 collisionPoint = collision.GetContact(0).point;
            GameObject spark = Instantiate(hitSparkles, collisionPoint, Quaternion.identity);
            Destroy(spark, 3f);
            FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
            audioSource.PlayOneShot(hitSound);

            if(HP < 1)
            {
                if (!ded)
                {
                    animator.SetBool("isDefeated", true);

                    Invoke("playdrill", 1f);
                    FindObjectOfType<KillText>().getReportedTo();
                    hpSlider.gameObject.SetActive(false);
                    ded = true;
                    FindObjectOfType<FinalBossAssorter>().hideRamp = false;
                    destroyParticles.SetActive(true);
                    Invoke("nextPhase", 2.3f);
                    //defeat
                }
            }
        }
    }

    void nextPhase()
    {
        Instantiate(lordPuteyPrefab, transform.position + new Vector3(0,4,0), Quaternion.identity);
        Destroy(this.gameObject);
    }

    void Rotate()
    {
        if (rotateTowardsPlayer)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
            targetRotation *= Quaternion.Euler(0f, 180, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);

            if(turnSource.isPlaying)
            turnSource.Play();
        }
        else
        {
            turnSource.Stop();
        }
    }

    void Movement()
    {
        if (isDriving)
        {
            Vector3 moveDirection = visual.transform.right * 5;

            rb.velocity = moveDirection;
            exhaust.enableEmission = true;

            if (!driveSource.isPlaying)
                driveSource.Play();
        }
        else
        {
            exhaust.enableEmission = false;

            driveSource.Stop();
        }
    }

    void ToggleMovement()
    {
        int r = Random.Range(5, 10);
        isDriving = !isDriving;

        if (isDriving)
        {
            animator.SetBool("isDriving", true);
        }
        else
        {
            animator.SetBool("isDriving", false);
        }
        Invoke("ToggleMovement", r);
    }

    void ToggleRotateTowardsPlayer()
    {
        int r = Random.Range(5, 10);
        rotateTowardsPlayer = !rotateTowardsPlayer;
        Invoke("ToggleRotateTowardsPlayer", r);
    }

    void PerformAttacks()
    {
        int r = Random.Range(5, 9);
        int r1 = Random.Range(2, 14);
        int r2 = Random.Range(2, 14);
        int r3 = Random.Range(2, 14);

        Invoke("invokeShootCannon", r1);
        Invoke("invokeShootMGuns", r2);
        Invoke("invokeShootGrenades", r3);
        
        Invoke("PerformAttacks", r);
    }

    public void GetTriggered()
    {
        ToggleMovement();
        ToggleRotateTowardsPlayer();
        PerformAttacks();

        hpSlider.gameObject.SetActive(true);
    }

    void invokeShootCannon()
    {
        rotateTowardsPlayer = true;
        StartCoroutine(ShootCannon());
    }

    void invokeShootMGuns()
    {
        StartCoroutine(ShootMGuns());
    }

    void invokeShootGrenades()
    {
        StartCoroutine(LaunchGrenades());
    }

    IEnumerator ShootCannon()
    {
        yield return new WaitForSeconds(0.5f);
        if (!ded)
        {
            audioSource.PlayOneShot(thump);
            GameObject grenade = Instantiate(explosiveBullet, cannonShotSpot.position, Quaternion.identity);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.useGravity = true;

            rb.AddForce(visual.transform.right * 5000 * Time.deltaTime, ForceMode.Impulse);

            yield return new WaitForSeconds(1);
            audioSource.PlayOneShot(reload);
        }
    }
    void playdrill()
    {
        audioSource.PlayOneShot(drill);
    }
    IEnumerator ShootMGuns()
    {
        int bulletsToShoot = 0;
        bulletsToShoot = Random.Range(6, 20);
        while (bulletsToShoot > 1)
        {
            if (!ded)
            {
                yield return new WaitForSeconds(0.22f);

                Vector3 directionToPlayer = player.position - mGun1hole.position;
                Vector3 directionToPlayer2 = player.position - mGun2hole.position;

                if (Vector3.Angle(mGun1hole.right, directionToPlayer) <= 90)
                {
                    GameObject bulletObject = Instantiate(mGunBullet, mGun1hole.position, Quaternion.identity);
                    directionToPlayer = ApplyBulletInaccuracy(directionToPlayer);
                    Destroy(bulletObject, 5f);
                    Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();
                    bulletRigidbody.velocity = directionToPlayer.normalized * 45;
                    audioSource.PlayOneShot(mGunShot);
                }
                bulletsToShoot--;
                yield return new WaitForSeconds(0.11f);

                if (Vector3.Angle(mGun2hole.right, directionToPlayer2) <= 90)
                {
                    GameObject bulletObject2 = Instantiate(mGunBullet, mGun2hole.position, Quaternion.identity);
                    Destroy(bulletObject2, 5f);
                    directionToPlayer2 = ApplyBulletInaccuracy(directionToPlayer2);
                    Rigidbody bulletRigidbody2 = bulletObject2.GetComponent<Rigidbody>();
                    bulletRigidbody2.velocity = directionToPlayer2.normalized * 45;
                    audioSource.PlayOneShot(mGunShot);
                }
            }
            bulletsToShoot--;
        }
    }

    private Vector3 ApplyBulletInaccuracy(Vector3 direction)
    {
        float horizontalInaccuracyAngle = Random.Range(-5, 5);
        float verticalInaccuracyAngle = Random.Range(-5, 5);

        Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalInaccuracyAngle, Vector3.up);
        direction = horizontalRotation * direction;

        Quaternion verticalRotation = Quaternion.AngleAxis(verticalInaccuracyAngle, Vector3.right);
        direction = verticalRotation * direction;

        return direction;
    }
}

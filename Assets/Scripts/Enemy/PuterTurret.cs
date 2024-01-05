using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuterTurret : MonoBehaviour
{
    public int health = 3;
    public GameObject bulletPrefab;
    public Transform player;
    public float bulletSpeed = 10f;
    public float accuracy = 0.1f;
    public float fireRate = 1f;  // Bullets per second
    public float detectionRange;
    public Transform muzzle1;
    public Transform muzzle2;
    public Transform muzzle3;
    public Transform muzzle4;
    public Transform muzzle5;
    public Transform muzzle6;
    public ParticleSystem muzzleFlare;
    public ParticleSystem muzzleFlare2;
    public ParticleSystem muzzleFlare3;
    public ParticleSystem muzzleFlare4;
    public ParticleSystem muzzleFlare5;
    public ParticleSystem muzzleFlare6;
    public AudioSource audioSource;
    public float bulletInaccuracy;
    public AudioClip[] gethitsounds, dieSounds;
    public AudioClip shootsound;
    public GameObject explosionParts, sparks;
    public float minTimeBetweenShots = 0.1f;  // Adjust this value to set the minimum duration between individual shots
    public float maxTimeBetweenShots = 0.3f;  // Adjust this value to set the maximum duration between individual shots
    public float minTimeBetweenBursts = 2f;   // Adjust this value to set the minimum duration between bursts of fire
    public float maxTimeBetweenBursts = 4f;   // Adjust this value to set the maximum duration between bursts of fire
    private float nextShotTime;
    private float nextBurstTime;
    private float nextFireTime;
    public float shootingRange;
    bool isTriggered;
    public float rotationSpeed;
    public AudioClip[] gunShots;
    public AudioClip[] damageSounds;
    public AudioSource hitSounds;
    public AudioClip targetedSound;
    public GameObject explosionParticles;
    public GameObject collisionParticles;
    public GameObject explosionForce;
    bool ded;

    public GameObject happy1;
    public GameObject happy2;
    public GameObject happy3;
    public GameObject angry1;
    public GameObject angry2;
    public GameObject angry3;
    
    public MeshRenderer screen;
    public MeshRenderer screen2;
    public MeshRenderer screen3;
    public Material blueScreen;

    private void Start()
    {

        nextBurstTime = Time.time + Random.Range(minTimeBetweenBursts, maxTimeBetweenBursts);
        nextShotTime = Time.time;
        player = FindObjectOfType<Player>().GetComponent<Transform>();
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
            if (player != null)
            {
                Vector3 targetDirection = (player.position - transform.position).normalized;
                Quaternion desiredRotation = Quaternion.LookRotation(targetDirection);

                // Add inaccuracy to the desired rotation
                float randomAngleY = Random.Range(-accuracy, accuracy);
                float randomAngleZ = Random.Range(-accuracy, accuracy);
                float randomAngleX = Random.Range(-accuracy, accuracy);
                desiredRotation *= Quaternion.Euler(randomAngleX, randomAngleY, randomAngleZ);

                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime);

                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                RaycastHit hit;
                Vector3 rayDirection = player.position - transform.position;
                Physics.Raycast(transform.position, rayDirection, out hit, detectionRange);
                if (distanceToPlayer <= shootingRange && hit.collider.CompareTag("Player"))
                {
                    // Player is within shooting range, start shooting
                    if (Time.time >= nextShotTime)
                    {
                        ShootAtPlayer();
                        nextShotTime = Time.time + Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
                    }
                }
            }
        }
    }
    public void takeDamage()
    {
        health--;
        FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
        if (health < 1)
        {
            blueScreenAll();
            GameObject expo = Instantiate(explosionForce, transform.position, Quaternion.identity);
            Destroy(expo, 5f);
            ded = true;
            int randomIndex = Random.Range(0, dieSounds.Length);
            AudioClip hitSound = dieSounds[randomIndex];
            audioSource.clip = hitSound;
            audioSource.PlayOneShot(audioSource.clip);
            GameObject oof = Instantiate(explosionParticles, transform.position, Quaternion.identity);
            Destroy(oof, 5f);
            //GameObject lighty = Instantiate(orangeLight, collisionPoint, Quaternion.identity);
            //Destroy(lighty, 0.25f);
            FindObjectOfType<KillText>().getReportedTo();
            ScreenShake.Shake(0.25f, 0.05f);
            Destroy(GetComponent<BoxCollider>());
            Destroy(this.gameObject, 4.2f);
            Destroy(this); // add change to ragdoll
                           //orangeLight.gameObject.SetActive(false);
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
    private void OnCollisionEnter(Collision collision)
    {
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
                        //if (Random.value < 0.25f)
                        //{
                        //    audioSource.PlayOneShot(letsGetItOn);
                        //}
                        isTriggered = true;
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
                    blueScreenAll();
                    GameObject expo = Instantiate(explosionForce, transform.position, Quaternion.identity);
                    Destroy(expo, 5f);
                    ded = true;
                    int randomIndex = Random.Range(0, dieSounds.Length);
                    AudioClip hitSound = dieSounds[randomIndex];
                    audioSource.clip = hitSound;
                    audioSource.PlayOneShot(audioSource.clip);
                    Vector3 collisionPoint = collision.GetContact(0).point;
                    GameObject oof = Instantiate(explosionParticles, collisionPoint, Quaternion.identity);
                    Destroy(oof, 5f);
                    //GameObject lighty = Instantiate(orangeLight, collisionPoint, Quaternion.identity);
                    //Destroy(lighty, 0.25f);
                    FindObjectOfType<KillText>().getReportedTo();
                    ScreenShake.Shake(0.25f, 0.05f);
                    Destroy(GetComponent<BoxCollider>());
                    Destroy(this.gameObject, 4.2f);
                    Destroy(this); // add change to ragdoll
                    //orangeLight.gameObject.SetActive(false);
                    // Add rigidbody to each child GameObject and apply random torqued force
                    foreach (Transform child in transform)
                    {
                        if (child.gameObject.activeInHierarchy)
                        {
                            Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();
                            Destroy(child.gameObject, 4f);

                            Vector3 randomForce = Random.onUnitSphere * Random.Range(2f, 5f);
                            Vector3 randomTorque = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));

                            if (childRigidbody)
                            {
                                childRigidbody.AddForce(randomForce, ForceMode.Impulse);
                                childRigidbody.AddTorque(randomTorque, ForceMode.Impulse);
                            }
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
    }


    private void ShootAtPlayer()
    {
        if (Time.time >= nextBurstTime)
        {
            // Start of a new burst, update the nextBurstTime
            nextBurstTime = Time.time + Random.Range(minTimeBetweenBursts, maxTimeBetweenBursts);
        }

        int randomIndex = Random.Range(0, 6);
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
                if (randomIndex == 1)
                {
                    GameObject bullet = Instantiate(bulletPrefab, muzzle2.position, Quaternion.identity);
                    Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
                    bulletRB.velocity = bulletDirection * bulletSpeed;
                    muzzleFlare2.Play();
                }
                if (randomIndex == 2)
                {
                    GameObject bullet = Instantiate(bulletPrefab, muzzle3.position, Quaternion.identity);
                    Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
                    bulletRB.velocity = bulletDirection * bulletSpeed;
                    muzzleFlare3.Play();
                }
                if (randomIndex == 3)
                {
                    GameObject bullet = Instantiate(bulletPrefab, muzzle4.position, Quaternion.identity);
                    Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
                    bulletRB.velocity = bulletDirection * bulletSpeed;
                    muzzleFlare4.Play();
                }
                if (randomIndex == 4)
                {
                    GameObject bullet = Instantiate(bulletPrefab, muzzle5.position, Quaternion.identity);
                    Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
                    bulletRB.velocity = bulletDirection * bulletSpeed;
                    muzzleFlare5.Play();
                }
                if (randomIndex == 5)
                {
                    GameObject bullet = Instantiate(bulletPrefab, muzzle6.position, Quaternion.identity);
                    Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
                    bulletRB.velocity = bulletDirection * bulletSpeed;
                    muzzleFlare6.Play();
                }

        int r2 = Random.Range(0, gunShots.Length);
        AudioClip sound = gunShots[r2];
        audioSource.PlayOneShot(sound);

        nextShotTime = Time.time + Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    private IEnumerator ToggleObjects()
    {
        while (!ded)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            int randomNumber = Random.Range(1, 4);

            // Toggle objects based on the randomly generated number
            switch (randomNumber)
            {
                case 1:
                    ToggleGameObject(happy1, angry1);
                    break;
                case 2:
                    ToggleGameObject(happy2, angry2);
                    break;
                case 3:
                    ToggleGameObject(happy3, angry3);
                    break;
            }
        }
    }

    private void blueScreenAll()
    {
        happy1.gameObject.SetActive(false);
        happy2.gameObject.SetActive(false);
        happy3.gameObject.SetActive(false);
        angry1.gameObject.SetActive(false);
        angry2.gameObject.SetActive(false);
        angry3.gameObject.SetActive(false);

        screen.material = blueScreen;
        screen2.material = blueScreen;
        screen3.material = blueScreen;
    }
    private void ToggleGameObject(GameObject happyObj, GameObject angryObj)
    {
        happyObj.SetActive(!happyObj.activeSelf);
        angryObj.SetActive(!angryObj.activeSelf);
    }
}
   



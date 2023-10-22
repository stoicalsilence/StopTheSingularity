using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingBot : MonoBehaviour
{
    public int health = 3;
    public GameObject bulletPrefab;
    public Transform playerTransform;
    public float bulletSpeed = 10f;
    public float accuracy = 0.1f;
    public float fireRate = 1f;  // Bullets per second
    public Renderer head, body, cap;
    public float flashDuration = 0.2f;
    public Color flashColor = Color.red;
    private Color originalColorHead, originalColorBodyAndCap;
    public Transform muzzle;
    public AudioSource audioSource;
    public AudioClip[] gethitsounds, dieSounds;
    public AudioClip shootsound;
    public GameObject explosionParts, sparks, light;

    private float nextFireTime;

    private void Start()
    {
        nextFireTime = Time.time;
        originalColorBodyAndCap = body.material.color;
        originalColorHead = head.material.color;
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            Vector3 targetDirection = (playerTransform.position - transform.position).normalized;
            Quaternion desiredRotation = Quaternion.LookRotation(targetDirection);

            // Add inaccuracy to the desired rotation
            float randomAngleY = Random.Range(-accuracy, accuracy);
            float randomAngleZ = Random.Range(-accuracy, accuracy);
            float randomAngleX = Random.Range(-accuracy, accuracy);
            desiredRotation *= Quaternion.Euler(randomAngleX, randomAngleY, randomAngleZ);

            

            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime);

            if (Time.time >= nextFireTime)
            {
                // Instantiate bullet without any rotation
                GameObject bullet = Instantiate(bulletPrefab, muzzle.transform.position, Quaternion.identity);
                Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

                // Apply bullet spread to the desired rotation
                Quaternion bulletRotation = Quaternion.Euler(randomAngleX, randomAngleY, randomAngleZ) * transform.rotation;
                bullet.transform.rotation = bulletRotation;

                // Set the bullet's velocity based on the modified rotation
                bulletRigidbody.velocity = bullet.transform.forward * bulletSpeed;

                nextFireTime = Time.time + 1f / fireRate; // Calculate next fire time based on fire rate
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerAttack"))
        {
            TakeDamage();
            FlashColor();
            FindObjectOfType<HitmarkerEffect>().ShowHitmarker();

            if (health > 0)
            {
                Vector3 collisionPoint = collision.GetContact(0).point;
                GameObject ded = Instantiate(sparks, collisionPoint, Quaternion.identity);
                Destroy(ded, 5f);
            }
            else
            {
                Vector3 collisionPoint = collision.GetContact(0).point;
                GameObject oof = Instantiate(explosionParts, collisionPoint, Quaternion.identity);
                Destroy(oof, 5f);
                GameObject lighty = Instantiate(light, collisionPoint, Quaternion.identity);
                Destroy(lighty, 0.25f);
            }
        }
    }
        private void TakeDamage()
    {
        health--;
        if (gethitsounds.Length > 0)
        {
            FlashColor();
            int randomIndex = Random.Range(0, gethitsounds.Length);
            AudioClip hitSound = gethitsounds[randomIndex];
            audioSource.clip = hitSound;
            audioSource.PlayOneShot(audioSource.clip);
            ScreenShake.Shake(0.25f, 0.025f);
        }
        if (health <= 0)
        {
            if (dieSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, dieSounds.Length);
                AudioClip hitSound = dieSounds[randomIndex];
                audioSource.clip = hitSound;
                audioSource.PlayOneShot(audioSource.clip);
            }
            resetColor();
            FindObjectOfType<KillText>().getReportedTo();
            ScreenShake.Shake(0.25f, 0.05f);
            Destroy(this);
        }
    }

    private void FlashColor()
    {
        StartCoroutine(FlashColorCoroutine());
    }

    private IEnumerator FlashColorCoroutine()
    {
        head.material.color = flashColor;
        cap.material.color = flashColor;
        body.material.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        resetColor();
    }

    private void resetColor()
    {
        head.material.color = originalColorHead;
        body.material.color = originalColorBodyAndCap;
        cap.material.color = originalColorBodyAndCap;
    }
}
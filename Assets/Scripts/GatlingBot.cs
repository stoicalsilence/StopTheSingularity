using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingBot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform playerTransform;
    public float bulletSpeed = 10f;
    public float accuracy = 0.1f;
    public float fireRate = 1f;  // Bullets per second

    public Transform muzzle;

    private float nextFireTime;

    private void Start()
    {
        nextFireTime = Time.time;
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float cooldownTime = 0.5f;
    public float bulletSpeed = 20f;
    private float cooldownTimer = 0f;
    public bool reloading;
    public float reloadTime;
    public float reloadTimeOnEmpty;

    public int magCapacity;
    public int ammoInMag;
    public int maxAmmo;
    public Transform shootHole;

    public AudioSource gunShotSound;
    public AudioClip[] gunShots;

    public AudioClip[] bulletInserts;
    public AudioClip[] shotgunPumps;
    public AudioClip[] noAmmoClick;
    public Animator anim;

    public ParticleSystem muzzleFlash;
    public GameObject muzzleLight;
    private bool isNoAmmoSoundPlayed = false;

    public AnimationClip idleAnim;
    public AnimationClip fireAnim;
    public AnimationClip fireAnim2;
    public AnimationClip reloadStart;
    public AnimationClip reloadOne;
    public AnimationClip reloadEnd;
    private bool stoprel;
    public float recoilForce = 5f;
    Vector3 recoilDirection;

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
    public void Shoot()
    {

        if (cooldownTimer <= 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(100f);
            }

            if (ammoInMag > 0)
            {
                StopAllCoroutines();
                reloading = false;
                StartCoroutine(shootAnimation());
                ammoInMag--;
                int randomIndex = Random.Range(0, gunShots.Length);
                AudioClip sound = gunShots[randomIndex];
                gunShotSound.volume = 0.3f;
                gunShotSound.PlayOneShot(sound);
                gunShotSound.volume = 0.5f;
                for (int i = 0; i < 6; i++)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shootHole.position, transform.rotation);
                    
                    
                    Vector3 direction = (targetPoint - shootHole.position).normalized;
                    direction = Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f) * direction;
                    direction.Normalize();
                    recoilDirection = -direction;
                    Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

                    float bulletSpeedWithInaccuracy = bulletSpeed * Random.Range(0.9f, 1.1f);
                    bulletRigidbody.AddForce(direction * bulletSpeedWithInaccuracy, ForceMode.VelocityChange);
                }
                FindObjectOfType<Player>().GetComponent<Rigidbody>().AddForce(recoilDirection * recoilForce, ForceMode.Impulse);

                cooldownTimer = fireAnim.length;
            }
            else if (ammoInMag == 0 && !isNoAmmoSoundPlayed)
            {
                int randomIndex = Random.Range(0, noAmmoClick.Length);
                AudioClip sound = noAmmoClick[randomIndex];
                gunShotSound.PlayOneShot(sound);
                isNoAmmoSoundPlayed = true;
            }
        }
        else
        {
            isNoAmmoSoundPlayed = false;
        }
    }

    public IEnumerator shootAnimation()
    {
        stoprel = true;
        ScreenShake.Shake(0.1f, 0.25f);
        float randomValue = Random.value;
        AnimationClip chosenAnimation = randomValue < 0.5f ? fireAnim : fireAnim2;
        anim.Play(chosenAnimation.name);
        muzzleFlash.Play();
        muzzleLight.SetActive(true);
        yield return new WaitForSeconds(0.16f);
        muzzleLight.SetActive(false);
        int randomIndex = Random.Range(0, shotgunPumps.Length);
        AudioClip sound = shotgunPumps[randomIndex];
        gunShotSound.PlayOneShot(sound);
        stoprel = false;
    }

    public IEnumerator reload()
    {
        reloading = true;
        anim.Play(reloadStart.name);
        yield return new WaitForSeconds(reloadStart.length);
        while (ammoInMag < magCapacity)
        {
            if (ammoInMag < magCapacity)
            {
                anim.Play(reloadOne.name);
                int randomIndex = Random.Range(0, bulletInserts.Length);
                AudioClip sound = bulletInserts[randomIndex];
                gunShotSound.PlayOneShot(sound);
                ammoInMag++;
            }
            yield return new WaitForSeconds(reloadOne.length);
            if(ammoInMag == magCapacity)
            {
                anim.Play(reloadEnd.name);
                gunShotSound.volume = 0.5f;
                int randomIndex3 = Random.Range(0, shotgunPumps.Length);
                AudioClip sound3 = shotgunPumps[randomIndex3];
                gunShotSound.PlayOneShot(sound3);
                yield return new WaitForSeconds(reloadEnd.length);
            }
        }
        reloading = false;
    }

   

    public void playCockingNoise()
    {
        gunShotSound.volume = 0.5f;
        int randomIndex3 = Random.Range(0, shotgunPumps.Length);
        AudioClip sound3 = shotgunPumps[randomIndex3];
        gunShotSound.PlayOneShot(sound3);
    }
}

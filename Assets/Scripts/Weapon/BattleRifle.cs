using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRifle : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float cooldownTime = 0.5f;
    public float bulletSpeed = 20f;
    public float cooldownTimer = 0f;
    public bool reloading;
    public float reloadTime;
    public float reloadTimeOnEmpty;

    public int magCapacity;
    public int ammoInMag;
    public int maxAmmo;
    public Transform shootHole;

    public AudioSource gunShotSound;
    public AudioClip[] gunShots;

    public AudioClip[] magReleases;
    public AudioClip[] magInserts;
    public AudioClip[] gunCocking;
    public AudioClip[] noAmmoClick;
    public Animator anim;

    public ParticleSystem muzzleFlash;
    public GameObject muzzleLight;
    private bool isNoAmmoSoundPlayed = false;

    public AnimationClip idleAnim;
    public AnimationClip fireAnim;
    public AnimationClip reloadAnim;
    public AnimationClip reloadOnEmptyAnim;
    public ParticleSystem muzzleSmoke, muzzleSmoke1;
    private bool isMuzzleSmokeActive = false;
    private Coroutine disableMuzzleSmokeCoroutine;
    private int bulletCounter = 0;
    private int bulletsThreshold = 3;
    private float timeWindow = 2f; // Time window in seconds

    private float lastFireTime;
    public float minInnacurracy = 0f;
    public float maxInnacurracy = 0.025f;

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
    public void Shoot()
    {
        if (cooldownTimer <= 0f && !reloading)
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
                StartCoroutine(shootAnimation());
                if (Time.time - lastFireTime > timeWindow)
                {
                    // Reset the bullet counter if the time window has expired
                    bulletCounter = 0;
                }

                bulletCounter++;
                lastFireTime = Time.time;

                if (bulletCounter >= bulletsThreshold && !isMuzzleSmokeActive)
                {
                    enableMuzzleSmoke();
                }
                ammoInMag--;
                GameObject bullet = Instantiate(bulletPrefab, shootHole.position, transform.rotation);
                int randomIndex = Random.Range(0, gunShots.Length);
                AudioClip sound = gunShots[randomIndex];
                gunShotSound.volume = 0.1f;
                gunShotSound.PlayOneShot(sound);
                gunShotSound.volume = 0.5f;
                Vector3 direction = (targetPoint - shootHole.position).normalized;

                Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
                float inaccuraccy = Random.Range(minInnacurracy, maxInnacurracy);

                // Apply random recoil to the direction vector
                Vector3 recoilVector = Random.onUnitSphere * inaccuraccy;
                direction += recoilVector;

                // Normalize the modified direction
                direction.Normalize();

                // Apply force to the bullet with modified direction and bulletSpeed
                bulletRigidbody.AddForce(direction * bulletSpeed, ForceMode.VelocityChange);
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
        ScreenShake.Shake(0.1f, 0.1f);
        anim.Play(fireAnim.name);
        muzzleFlash.Play();
        muzzleLight.SetActive(true);
        yield return new WaitForSeconds(0.16f);
        muzzleLight.SetActive(false);
    }
    public void enableMuzzleSmoke()
    {
        if (isMuzzleSmokeActive)
        {
            if (disableMuzzleSmokeCoroutine != null)
            {
                StopCoroutine(disableMuzzleSmokeCoroutine);
            }
        }
        else
        {
            muzzleSmoke.Play();
            muzzleSmoke1.Play();
            isMuzzleSmokeActive = true;
        }

        disableMuzzleSmokeCoroutine = StartCoroutine(DisableMuzzleSmokeAfterDelay(3f));
    }

    private IEnumerator DisableMuzzleSmokeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        disableMuzzleSmoke();
    }

    public void disableMuzzleSmoke()
    {
        muzzleSmoke.Stop();
        muzzleSmoke1.Stop();
        isMuzzleSmokeActive = false;
    }

    public IEnumerator reload()
    {
        gunShotSound.volume = 0.5f;
        reloading = true;
        anim.Play(reloadAnim.name);
        yield return new WaitForSeconds(reloadAnim.length / 2);
        int randomIndex = Random.Range(0, magReleases.Length);
        AudioClip sound = magReleases[randomIndex];
        gunShotSound.PlayOneShot(sound);
        yield return new WaitForSeconds(reloadAnim.length / 2);
        int randomIndex2 = Random.Range(0, magInserts.Length);
        AudioClip sound2 = magInserts[randomIndex2];
        gunShotSound.PlayOneShot(sound2);
        ammoInMag = magCapacity;
        reloading = false;
    }

    public IEnumerator reloadOnEmpty()
    {
        gunShotSound.volume = 0.5f;
        reloading = true;
        anim.Play(reloadOnEmptyAnim.name);
        yield return new WaitForSeconds(reloadOnEmptyAnim.length / 3);
        int randomIndex = Random.Range(0, magReleases.Length);
        AudioClip sound = magReleases[randomIndex];
        gunShotSound.PlayOneShot(sound);
        yield return new WaitForSeconds(reloadOnEmptyAnim.length / 3);
        int randomIndex2 = Random.Range(0, magInserts.Length);
        AudioClip sound2 = magInserts[randomIndex2];
        gunShotSound.PlayOneShot(sound2);
        yield return new WaitForSeconds(reloadOnEmptyAnim.length / 3);
        int randomIndex3 = Random.Range(0, gunCocking.Length);
        AudioClip sound3 = gunCocking[randomIndex3];
        gunShotSound.PlayOneShot(sound3);
        ammoInMag = magCapacity;
        reloading = false;
    }

    public void playCockingNoise()
    {
        gunShotSound.volume = 0.5f;
        int randomIndex3 = Random.Range(0, gunCocking.Length);
        AudioClip sound3 = gunCocking[randomIndex3];
        gunShotSound.PlayOneShot(sound3);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DblBarrelShotgun : MonoBehaviour
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
    public AnimationClip reloadOne;
    public AnimationClip reloadAll;
    public bool singleShot;
    public float recoilForce = 15f;
    Vector3 recoilDirection;

    private bool isMuzzleSmokeActive = false;
    private Coroutine disableMuzzleSmokeCoroutine;
    public ParticleSystem muzzleSmoke;
    public ParticleSystem muzzleSmoke1;
    public ParticleSystem reloadSmoke;
    public ParticleSystem reloadSmoke1;

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
                enableMuzzleSmoke();
                StartCoroutine(shootAnimation());
                ammoInMag--;
                int randomIndex = Random.Range(0, gunShots.Length);
                AudioClip sound = gunShots[randomIndex];
                gunShotSound.volume = 0.3f;
                gunShotSound.PlayOneShot(sound);
                gunShotSound.volume = 0.5f;

                int ammountofbullets;
                if (singleShot)
                {
                    ammountofbullets = 10;
                    recoilForce = 10;
                }
                else
                {
                    ammountofbullets = 20;
                    recoilForce = 20;
                }
                for (int i = 0; i < ammountofbullets; i++)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shootHole.position, transform.rotation);


                    Vector3 direction = (targetPoint - shootHole.position).normalized;
                    direction = Quaternion.Euler(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f) * direction;
                    direction.Normalize();
                    recoilDirection = -direction;
                    Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

                    float bulletSpeedWithInaccuracy = bulletSpeed * Random.Range(0.6f, 1.4f);
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

    public void disableReloadSmoke()
    {
        reloadSmoke.Stop();
        reloadSmoke1.Stop();
    }
    public IEnumerator shootAnimation()
    {
        gunShotSound.volume = 0.5f;
        ScreenShake.Shake(0.1f, 0.25f);
        AnimationClip chosenAnimation;
        if (singleShot)
        {
            chosenAnimation = fireAnim;
        }
        else
        {
            chosenAnimation = fireAnim2;
        }
        anim.Play(chosenAnimation.name);
        muzzleFlash.Play();
        muzzleLight.SetActive(true);
        yield return new WaitForSeconds(0.16f);
        muzzleLight.SetActive(false);
    }

    public IEnumerator reload()
    {
        reloading = true;
            if (ammoInMag == 1)
            {
                anim.Play(reloadOne.name);
            reloadSmoke.Play();
            Invoke("disableReloadSmoke", 1.5f);
            int randomIndex3 = Random.Range(0, shotgunPumps.Length);
            AudioClip sound3 = shotgunPumps[randomIndex3];
            gunShotSound.PlayOneShot(sound3);
            yield return new WaitForSeconds(reloadOne.length);
            ammoInMag = magCapacity;
        }
            if (ammoInMag == 0)
            {
                anim.Play(reloadAll.name);
            reloadSmoke1.Play();
            reloadSmoke.Play();
            Invoke("disableReloadSmoke", 1.5f);
            gunShotSound.volume = 0.5f;
                int randomIndex3 = Random.Range(0, shotgunPumps.Length);
                AudioClip sound3 = shotgunPumps[randomIndex3];
                gunShotSound.PlayOneShot(sound3);
            yield return new WaitForSeconds(reloadAll.length);
            ammoInMag = magCapacity;
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

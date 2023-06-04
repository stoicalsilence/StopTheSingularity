using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firearm : MonoBehaviour
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

    public AudioClip[] magReleases;
    public AudioClip[] magInserts;
    public AudioClip[] gunCocking;
    public AudioClip[] noAmmoClick;
    public Animator anim;

    public ParticleSystem muzzleFlash;
    public GameObject muzzleLight;
    private bool isNoAmmoSoundPlayed = false;

    public AnimationClip slideAnim;
    public AnimationClip reloadAnim;
    public AnimationClip reloadOnEmptyAnim;

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
            if (ammoInMag > 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 targetPoint = hit.point;
                    StartCoroutine(shootAnimation());
                    ammoInMag--;
                    GameObject bullet = Instantiate(bulletPrefab, shootHole.position, transform.rotation);
                    int randomIndex = Random.Range(0, gunShots.Length);
                    AudioClip sound = gunShots[randomIndex];
                    gunShotSound.volume = 0.5f;
                    gunShotSound.PlayOneShot(sound);
                    gunShotSound.volume = 1f;
                    Vector3 direction = (targetPoint - shootHole.position).normalized;

                    Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

                    bulletRigidbody.AddForce(direction * bulletSpeed, ForceMode.VelocityChange);

                    cooldownTimer = cooldownTime;
                }
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
        anim.Play(slideAnim.name);
        muzzleFlash.Play();
        muzzleLight.SetActive(true);
        yield return new WaitForSeconds(0.16f);
        muzzleLight.SetActive(false);
    }

    public IEnumerator reload()
    {
        
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
        int randomIndex3 = Random.Range(0, gunCocking.Length);
        AudioClip sound3 = gunCocking[randomIndex3];
        gunShotSound.PlayOneShot(sound3);
    }
}

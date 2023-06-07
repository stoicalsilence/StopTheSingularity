using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public int maxHP = 100;
    public Animator swordAnim;
    public bool slashOnCD;
    public bool isBlocking;
    public bool isInvincible;
    public float invincibilityTime;
    public PlayerMovement playerMovement;
    public GameObject swordTrail;
    public GameObject swordPokeTrail;
    public Collider swordCollider;
    public AudioSource swordSwoosh;
    public AudioSource painAudio;
    public AudioSource blockSound;
    public AudioClip[] swordSwooshSounds;
    public AudioClip[] lowSwordSwooshSounds;
    public AudioClip[] lowPainSounds;
    public AudioClip[] highPainSounds;
    public AudioClip[] blockSounds;
    public Slider progressBar;
    public float cooldownDuration = 1.0f;
    public PostProcessProfile postProcessProfile;
    private Vignette vignette;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public GameObject sliderFill;

    private float cooldownTimer = 0.0f;

    public bool swordEquipped;
    public bool grapplingGunEquipped;
    public bool glockEquipped;
    public bool icePickEquipped;
    public bool assaultrifleEquipped;
    public bool shotgunEquipped;
    public GameObject grapplingGun;
    public GameObject sword;
    public Firearm glock;
    public TextMeshProUGUI ammoText;
    public GameObject icePick;
    public AssaultRifle assaultRifle;
    public Shotgun shotgun;

    public AudioClip swordUnsheath;
    public AudioClip icePickUnStick;

    public AudioClip[] icePickStickSounds;
    public AudioSource icePickSound;

    public GameObject icePickParticles;

    float minimumSpeedThreshold = 8.0f;
    float maximumSpeed = 30.0f;
    public AudioSource windSoundEffect;
    public AudioSource slideAudioSource;

    public AudioClip[] strongLandingSounds;
    public AudioClip[] normalLandingSounds;
    float fallThresholdVelocity = 8;
    bool wasFalling;
    bool wasJumping;
    public AudioClip jumpSound;
    public AudioClip[] slideSounds;
    public AudioClip[] slideStops;
    public GameObject jumpParticles;
    public GameObject largeDropParticles;
    public GameObject largeDropParticles2;
    public Transform jumpParticlePos;

    public GameObject plasmatana; 
    public AnimationClip hitAnimation1, hitAnimation2; 
    public Animator plasmatanaAnimation; 
    public TrailRenderer plasmatanaTrails, plasmatanaTrails2;
    public bool plasmatanaReady;
    public bool attackingWithPlasmatana;
    public GameObject ItemHolder;

    // Start is called before the first frame update
    void Start()
    {
        plasmatanaAnimation = plasmatana.GetComponent<Animator>();
        hpSlider.maxValue = maxHP;
        vignette = postProcessProfile.GetSetting<Vignette>();
        plasmatanaReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.V))
        //{
        //    startSlowmotion();
        //    Invoke("stopSlowmotion", 2f);
        //}

        if (Input.GetKeyDown(KeyCode.V) && plasmatanaReady)
        {
            StartCoroutine(plasmatanaAttack());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            equipSword();

            unequipGlock();
            unequipGrapplingGun();
            unequipIcePick();
            unequipAssaultRifle();
            unequipShotgun();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            equipGlock();

            unequipSword();
            unequipGrapplingGun();
            unequipIcePick();
            unequipAssaultRifle();
            unequipShotgun();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            equipGrapplingGun();

            unequipSword();
            unequipGlock();
            unequipIcePick();
            unequipAssaultRifle();
            unequipShotgun();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            equipIcePick();

            unequipSword();
            unequipGlock();
            unequipGrapplingGun();
            unequipAssaultRifle();
            unequipShotgun();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            equipAR();

            unequipSword();
            unequipGlock();
            unequipGrapplingGun();
            unequipIcePick();
            unequipShotgun();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            equipShotgun();

            unequipSword();
            unequipGlock();
            unequipGrapplingGun();
            unequipIcePick();
            unequipAssaultRifle();
        }

        hpSlider.value = HP;
        hpText.text = HP.ToString() + " %";

        if (glockEquipped)
        {
            ammoText.text = "Glock\n" + glock.ammoInMag.ToString() + " / " + glock.magCapacity.ToString();
        }
        else if (assaultrifleEquipped)
        {
            ammoText.text = "Assault Rifle\n" + assaultRifle.ammoInMag.ToString() + " / " + assaultRifle.magCapacity.ToString();
        }
        else if (shotgunEquipped)
        {
            ammoText.text = "Shotgun\n" + shotgun.ammoInMag.ToString() + " / " + shotgun.magCapacity.ToString();
        }
        else
        {
            ammoText.text = "";
        }
        
        if (HP < 1)
        {
            sliderFill.gameObject.SetActive(false);
        }
        if (HP <= 0)
        {
            //Defeat();
        }

        if (slashOnCD)
        {
            progressBar.gameObject.SetActive(true);
            cooldownTimer += Time.deltaTime;

            float progress = Mathf.Clamp01(cooldownTimer / cooldownDuration);
            progressBar.value = progress;

            if (cooldownTimer >= cooldownDuration)
            {
                cooldownTimer = 0.0f;
                slashOnCD = false;
                progressBar.gameObject.SetActive(false);
            }
        }
        else
        {
            progressBar.gameObject.SetActive(false);
        }

        if (swordEquipped)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !slashOnCD && !isBlocking && playerMovement.state != PlayerMovement.MovementState.air)
            {
                StartCoroutine(swordAttack());
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && !slashOnCD && !isBlocking && playerMovement.state == PlayerMovement.MovementState.air)
            {
                StartCoroutine(swordJumpAttack());
            }

            if (Input.GetKeyDown(KeyCode.Mouse2) && !slashOnCD && !isBlocking)
            {
                StartCoroutine(swordPoke());
            }
        }

        if (glockEquipped)
        {
            if (Input.GetKey(KeyCode.Mouse0) && !glock.reloading)
            {
                glock.Shoot();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (glock.ammoInMag > 0 && !glock.reloading && glock.ammoInMag < glock.magCapacity && glock.maxAmmo > 0)
                {
                    StartCoroutine(glock.reload());
                }
                else if (glock.ammoInMag == 0 && !glock.reloading && glock.ammoInMag < glock.magCapacity && glock.maxAmmo > 0)
                {
                    StartCoroutine(glock.reloadOnEmpty());
                }
            }
        }

        if (assaultrifleEquipped)
        {
            if (Input.GetKey(KeyCode.Mouse0) && !assaultRifle.reloading)
            {
                assaultRifle.Shoot();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (assaultRifle.ammoInMag > 0 && !assaultRifle.reloading && assaultRifle.ammoInMag < assaultRifle.magCapacity && assaultRifle.maxAmmo > 0)
                {
                    StartCoroutine(assaultRifle.reload());
                }
                else if (assaultRifle.ammoInMag == 0 && !assaultRifle.reloading && assaultRifle.ammoInMag < assaultRifle.magCapacity && assaultRifle.maxAmmo > 0)
                {
                    StartCoroutine(assaultRifle.reloadOnEmpty());
                }
            }
        }

        if (shotgunEquipped)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                shotgun.Shoot();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (shotgun.ammoInMag < shotgun.magCapacity && !shotgun.reloading)
                {
                    StartCoroutine(shotgun.reload());
                }
            }
        }

        if (playerMovement.horizontalInput != 0 || playerMovement.verticalInput != 0)
        {
            swordAnim.SetBool("isRunning", true);
        }
        else
        {
            swordAnim.SetBool("isRunning", false);
        }

        if (playerMovement.state == PlayerMovement.MovementState.air)
        {
            swordAnim.SetBool("isJumping", true);
        }
        else
        {
            swordAnim.SetBool("isJumping", false);
        }

        if (swordEquipped)
        {
            if (Input.GetKey(KeyCode.Mouse1) && !slashOnCD)
            {
                isBlocking = true;
                swordAnim.SetBool("isBlocking", true);
                swordAnim.SetBool("isBlockIdling", true);
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                isBlocking = false;
                swordAnim.SetBool("isBlocking", false);
                swordAnim.SetBool("isBlockIdling", false);
            }
        }
        
        float currentSpeed = playerMovement.rb.velocity.magnitude;
        if (currentSpeed > minimumSpeedThreshold)
        {
            float normalizedSpeed = Mathf.Clamp01((currentSpeed - minimumSpeedThreshold) / (maximumSpeed - minimumSpeedThreshold));
            float targetVolume = Mathf.Lerp(0, 1, normalizedSpeed);
            windSoundEffect.volume = targetVolume;
        }
        else
        {
            windSoundEffect.volume = windSoundEffect.volume - Time.deltaTime;
        }

        if (playerMovement.rb.velocity.y < -fallThresholdVelocity/3 && !playerMovement.grounded)
        {
            wasJumping = true;
        }
        if (playerMovement.rb.velocity.y < -fallThresholdVelocity && !playerMovement.grounded)
        {
            wasFalling = true;
        }

        if (playerMovement.grounded && wasFalling)
        {
            int randomIndex = Random.Range(0, strongLandingSounds.Length);
            AudioClip sound = strongLandingSounds[randomIndex];
            painAudio.PlayOneShot(sound);
            ScreenShake.Shake(1f, 1f);
            spawnJumpParticles();
            wasFalling = false;
        }

        if (playerMovement.grounded && wasJumping && !wasFalling)
        {
            int randomIndex = Random.Range(0, normalLandingSounds.Length);
            AudioClip sound = normalLandingSounds[randomIndex];
            painAudio.PlayOneShot(sound);
            spawnLargeDropParticles();
            ScreenShake.Shake(0.25f, 0.010f);
            wasJumping = false;
        }
    }

    public IEnumerator plasmatanaAttack()
    {
        attackingWithPlasmatana = true;
        plasmatanaReady = false;
        plasmatana.SetActive(true);
        plasmatanaTrails.Clear();
        plasmatanaTrails2.Clear();

        int randomIndex = Random.Range(0, swordSwooshSounds.Length);
        AudioClip sound = swordSwooshSounds[randomIndex];
        blockSound.PlayOneShot(sound);

        float randomValue = Random.value;
        AnimationClip chosenAnimation = randomValue < 0.5f ? hitAnimation1 : hitAnimation2;
        plasmatanaAnimation.Play(chosenAnimation.name);
        ScreenShake.SmoothShake(chosenAnimation.length, 0.15f);
        yield return new WaitForSeconds(chosenAnimation.length);
        plasmatana.SetActive(false);
        plasmatanaReady = true;
        attackingWithPlasmatana = false;
    }

    IEnumerator swordAttack()
    {
        slashOnCD = true;
        swordCollider.enabled = true;
        swordAnim.SetBool("isAttacking", true);
        playRandomSwordWoosh();
        ScreenShake.SmoothShake(1f, .25f);
        yield return new WaitForSeconds(0.3f);
        swordTrail.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        swordAnim.SetBool("isAttacking", false);
        yield return new WaitForSeconds(0.35f);
        swordTrail.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        swordCollider.enabled = false;
    }

    IEnumerator swordJumpAttack()
    {
        slashOnCD = true;
        swordCollider.enabled = true;
        swordAnim.SetBool("isJumpAttacking", true);
        playRandomLowSwordWoosh();
        yield return new WaitForSeconds(0.1f);

        swordPokeTrail.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        swordAnim.SetBool("isJumpAttacking", false);
        yield return new WaitForSeconds(0.5f);
        swordCollider.enabled = false;
        swordPokeTrail.gameObject.SetActive(false);
    }

    IEnumerator swordPoke()
    {
        slashOnCD = true;
        swordCollider.enabled = true;
        swordAnim.SetBool("isPoking", true);
        playRandomSwordWoosh();
        yield return new WaitForSeconds(0.2f);
        swordPokeTrail.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        swordAnim.SetBool("isPoking", false);
        yield return new WaitForSeconds(0.4f);
        swordPokeTrail.gameObject.SetActive(false);
        swordCollider.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        }

        if (collision.gameObject.CompareTag("EnemyBody"))
        {
            Rigidbody playerRigidbody = GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                Vector3 pushDirection = transform.position - collision.transform.position;
                pushDirection.y = 0f;

                pushDirection = pushDirection.normalized;
                float playerPushForce = 20f;

                playerRigidbody.AddForce(pushDirection * playerPushForce, ForceMode.Impulse);

                if (!isBlocking)
                {
                    if (!isInvincible)
                    {
                        HP -= 10;
                        playerDamageSound(10);
                        StartCoroutine(ActivateInvincibility());
                        // Activate vignette briefly
                        vignette.enabled.value = true;
                        vignette.intensity.value = 0.5f;

                        // Coroutine to fade out vignette intensity
                        StartCoroutine(FadeOutVignette());
                    }
                }
                else
                {
                    playBlockSound();
                }
            }
        }
    }

    public void startSlowmotion()
    {
        TimeManager.doSlowmotion();
    }
    public void stopSlowmotion()
    {
        TimeManager.undoSlowmotion();
    }

    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    private void playRandomSwordWoosh()
    {
        if (swordSwooshSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, swordSwooshSounds.Length);
            AudioClip sound = swordSwooshSounds[randomIndex];
            swordSwoosh.PlayOneShot(sound);
        }
    }
    private void playRandomLowSwordWoosh()
    {
        if (lowSwordSwooshSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, lowSwordSwooshSounds.Length);
            AudioClip sound = lowSwordSwooshSounds[randomIndex];
            swordSwoosh.PlayOneShot(sound);
        }
    }

    private void playerDamageSound(int dmg)
    {
        if (dmg < 24)
        {
            int randomIndex = Random.Range(0, lowPainSounds.Length);
            AudioClip sound = lowPainSounds[randomIndex];
            painAudio.PlayOneShot(sound);
        }
        else
        {
            int randomIndex = Random.Range(0, highPainSounds.Length);
            AudioClip sound = highPainSounds[randomIndex];
            painAudio.PlayOneShot(sound);
        }
    }

    private void playBlockSound()
    {
        int randomIndex = Random.Range(0, blockSounds.Length);
        AudioClip sound = blockSounds[randomIndex];
        blockSound.PlayOneShot(sound);
    }

    public void playSlideSound()
    {
        slideAudioSource.volume = 0.3f;
    }
    public void muteSlideSound()
    {
        slideAudioSource.volume = 0f;
    }

    public void stopSlideSound()
    {
        slideAudioSource.Stop();
    }

    private IEnumerator FadeOutVignette()
    {
        // Wait for a brief moment
        yield return new WaitForSeconds(0.5f);

        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        float startIntensity = vignette.intensity.value;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            vignette.intensity.value = Mathf.Lerp(startIntensity, 0f, t);
            yield return null;
        }

        // Disable vignette
        vignette.enabled.value = false;
    }

    public void spawnJumpParticles()
    {
        GameObject p = Instantiate(jumpParticles, jumpParticlePos.position, Quaternion.identity);
        Destroy(p, 4f);
    }
    public void spawnLargeDropParticles()
    {
        GameObject p1 = Instantiate(largeDropParticles, jumpParticlePos.position, Quaternion.identity);
        GameObject p2 = Instantiate(largeDropParticles2, jumpParticlePos.position, Quaternion.identity);
        Destroy(p1, 4f);
        Destroy(p2, 4f);
    }

    public void playIcePickStickSound()
    {
        int randomIndex = Random.Range(0, icePickStickSounds.Length);
        AudioClip sound = icePickStickSounds[randomIndex];
        icePickSound.PlayOneShot(sound);
    }
    public void equipSword()
    {
        swordEquipped = true;
        sword.SetActive(true);
        swordSwoosh.PlayOneShot(swordUnsheath);
    }
    public void equipGlock()
    {
        glockEquipped = true;
        glock.gameObject.SetActive(true);
        glock.playCockingNoise();
        glock.anim.Play("GlockIdle");
    }
    public void equipGrapplingGun()
    {
        grapplingGunEquipped = true;
        grapplingGun.SetActive(true);
    }
    public void equipIcePick()
    {
        icePickEquipped = true;
        icePick.gameObject.SetActive(true);
    }
    public void equipAR()
    {
        assaultrifleEquipped = true;
        assaultRifle.gameObject.SetActive(true);
        assaultRifle.playCockingNoise();
    }
    public void equipShotgun()
    {
        shotgunEquipped = true;
        shotgun.gameObject.SetActive(true);
        shotgun.playCockingNoise();
    }
    public void unequipSword()
    {
        sword.SetActive(false);
        swordEquipped = false;
    }
    public void unequipGlock()
    {
        glock.muzzleLight.SetActive(false);
        glock.gameObject.SetActive(false);
        glockEquipped = false;
    }
    public void unequipGrapplingGun()
    {
        grapplingGun.GetComponent<GrapplingGun>().StopGrapple();
        grapplingGun.SetActive(false);
        grapplingGunEquipped = false;
    }
    public void unequipIcePick()
    {
        icePick.SetActive(false);
        icePickEquipped = false;
        playerMovement.UnstickFromWall();
    }
    public void unequipAssaultRifle()
    {
        assaultRifle.muzzleLight.SetActive(false);
        assaultRifle.gameObject.SetActive(false);
        assaultrifleEquipped = false;
    }
    public void unequipShotgun()
    {
        shotgun.StopAllCoroutines();
        shotgun.muzzleLight.SetActive(false);
        shotgun.gameObject.SetActive(false);
        shotgunEquipped = false;
    }
}

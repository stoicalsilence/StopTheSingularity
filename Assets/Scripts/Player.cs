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
    public bool dead;
    public GameObject gameOverPanel;
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
    public bool dblBarrelShotgunEquipped;
    public bool uziEquipped;
    public bool redDotRifleEquipped;
    public bool singleShotRifleEquipped;
    public bool grenadePistolEquipped;
    public GameObject grapplingGun;
    public GameObject sword;
    public Firearm glock;
    public TextMeshProUGUI ammoText;
    public GameObject icePick;
    public AssaultRifle assaultRifle;
    public AssaultRifle redDotRifle;
    public AssaultRifle singleShotRifle;
    public Shotgun shotgun;
    public DblBarrelShotgun dblBarrelShotgun;
    public Uzi uzi;
    public GrenadePistol grenadePistol;

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
    public AudioClip wallRunStartSound;
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
    public AudioClip slideInit;
    public AudioClip[] plasmaSwings;
    public AudioSource plamsatanasound;
    public GameObject windParticles;

    public PlayerInventory playerInventory;
    public bool hasPlasmatana;
    bool dedweponchecked = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        plasmatanaAnimation = plasmatana.GetComponent<Animator>();
        hpSlider.maxValue = maxHP;
        vignette = postProcessProfile.GetSetting<Vignette>();
        plasmatanaReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(HP < 1)
        {
            HP = 0;
            dead = true;
        }
        if (dead && playerMovement)
        {
            if (!dedweponchecked)
            {
                dedweponchecked = true;
                playerInventory.DropWeapon(playerInventory.activeSlot);
            }
            gameObject.transform.localScale = new Vector3(1, playerMovement.crouchYScale);
            gameOverPanel.gameObject.SetActive(true);
            playerMovement.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.F) && plasmatanaReady && hasPlasmatana)
        {
            StartCoroutine(plasmatanaAttack());
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
        else if (redDotRifleEquipped)
        {
            ammoText.text = "Assault Rifle\n" + redDotRifle.ammoInMag.ToString() + " / " + redDotRifle.magCapacity.ToString();
        }
        else if (singleShotRifleEquipped)
        {
            ammoText.text = "Assault Rifle\n" + singleShotRifle.ammoInMag.ToString() + " / " + singleShotRifle.magCapacity.ToString();
        }
        else if (shotgunEquipped)
        {
            ammoText.text = "Shotgun\n" + shotgun.ammoInMag.ToString() + " / " + shotgun.magCapacity.ToString();
        }
        else if (dblBarrelShotgunEquipped)
        {
            ammoText.text = "Shotgun\n" + dblBarrelShotgun.ammoInMag.ToString() + " / " + dblBarrelShotgun.magCapacity.ToString();
        }
        else if (uziEquipped)
        {
            ammoText.text = "Uzi\n" + uzi.ammoInMag.ToString() + " / " + uzi.magCapacity.ToString();
        }
        else if (grenadePistolEquipped)
        {
            ammoText.text = "Grenade Pistol\n" + grenadePistol.ammoInMag.ToString() + " / " + grenadePistol.magCapacity.ToString();
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
            if (Input.GetKeyDown(KeyCode.Mouse0) && !glock.reloading)
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

        if (redDotRifleEquipped)
        {
            if (Input.GetKey(KeyCode.Mouse0) && !redDotRifle.reloading)
            {
                redDotRifle.Shoot();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (redDotRifle.ammoInMag > 0 && !redDotRifle.reloading && redDotRifle.ammoInMag < redDotRifle.magCapacity && redDotRifle.maxAmmo > 0)
                {
                    StartCoroutine(redDotRifle.reload());
                }
                else if (redDotRifle.ammoInMag == 0 && !redDotRifle.reloading && redDotRifle.ammoInMag < redDotRifle.magCapacity && redDotRifle.maxAmmo > 0)
                {
                    StartCoroutine(redDotRifle.reloadOnEmpty());
                }
            }
        }

        if (singleShotRifleEquipped)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !singleShotRifle.reloading)
            {
                singleShotRifle.Shoot();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (singleShotRifle.ammoInMag > 0 && !singleShotRifle.reloading && singleShotRifle.ammoInMag < singleShotRifle.magCapacity && singleShotRifle.maxAmmo > 0)
                {
                    StartCoroutine(singleShotRifle.reload());
                }
                else if (singleShotRifle.ammoInMag == 0 && !singleShotRifle.reloading && singleShotRifle.ammoInMag < singleShotRifle.magCapacity && singleShotRifle.maxAmmo > 0)
                {
                    StartCoroutine(singleShotRifle.reloadOnEmpty());
                }
            }
        }

        if (shotgunEquipped)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !shotgun.reloading)
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

        if (dblBarrelShotgunEquipped)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !dblBarrelShotgun.reloading)
            {
                dblBarrelShotgun.singleShot = true;
                dblBarrelShotgun.Shoot();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && !dblBarrelShotgun.reloading && dblBarrelShotgun.ammoInMag == 2)
            {
                dblBarrelShotgun.singleShot = false;
                dblBarrelShotgun.Shoot();
                dblBarrelShotgun.ammoInMag = 0;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (dblBarrelShotgun.ammoInMag < dblBarrelShotgun.magCapacity && !dblBarrelShotgun.reloading)
                {
                    StartCoroutine(dblBarrelShotgun.reload());
                }
            }
        }

        if (uziEquipped)
        {
            if (Input.GetKey(KeyCode.Mouse0) && !uzi.reloading)
            {
                uzi.Shoot();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (uzi.ammoInMag > 0 && !uzi.reloading && uzi.ammoInMag < uzi.magCapacity && uzi.maxAmmo > 0)
                {
                    StartCoroutine(uzi.reload());
                }
                else if (uzi.ammoInMag == 0 && !uzi.reloading && uzi.ammoInMag < uzi.magCapacity && uzi.maxAmmo > 0)
                {
                    StartCoroutine(uzi.reloadOnEmpty());
                }
            }
        }

        if (grenadePistolEquipped)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0) && !grenadePistol.reloading)
            {
                grenadePistol.Shoot();
            }
            if (Input.GetKeyDown(KeyCode.R) && !grenadePistol.reloading && grenadePistol.ammoInMag < grenadePistol.magCapacity && grenadePistol.maxAmmo > 0)
            {
                StartCoroutine(grenadePistol.reload());
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
            windParticles.SetActive(true);
}
        else if (playerMovement.rb.velocity.magnitude > 0)
        {
            float targetVolume = windSoundEffect.volume - Time.deltaTime;
            windSoundEffect.volume = Mathf.Clamp01(targetVolume);
            if (windSoundEffect.volume == 0)
                windParticles.SetActive(false);
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
            ScreenShake.SmoothShake(1f, 1f);
            spawnJumpParticles();
            wasFalling = false;
        }

        if (playerMovement.grounded && wasJumping && !wasFalling)
        {
            int randomIndex = Random.Range(0, normalLandingSounds.Length);
            AudioClip sound = normalLandingSounds[randomIndex];
            painAudio.PlayOneShot(sound);
            spawnLargeDropParticles();
            ScreenShake.SmoothShake(0.25f, 0.010f);
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
        int e = Random.Range(0, plasmaSwings.Length);
        AudioClip d = plasmaSwings[e];
        plamsatanasound.PlayOneShot(d);
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

    public void playerDamageSound(int dmg)
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

    public void playStartSlideSound()
    {
        painAudio.PlayOneShot(slideInit);
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
        glock.gameObject.GetComponent<Animator>().enabled = true;
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
    public void equipRedDotAR()
    {
        redDotRifleEquipped = true;
        redDotRifle.gameObject.SetActive(true);
        redDotRifle.playCockingNoise();
    }
    public void equipScopeAR()
    {
        singleShotRifleEquipped= true;
        singleShotRifle.gameObject.SetActive(true);
        singleShotRifle.playCockingNoise();
    }
    public void equipShotgun()
    {
        shotgunEquipped = true;
        shotgun.gameObject.SetActive(true);
        shotgun.playCockingNoise();
    }
    public void equipDblBarrelShotgun()
    {
        dblBarrelShotgunEquipped = true;
        dblBarrelShotgun.gameObject.SetActive(true);
        dblBarrelShotgun.playCockingNoise();
    }
    public void equipUzi()
    {
        uziEquipped = true;
        uzi.gameObject.SetActive(true);
        uzi.playCockingNoise();
    }
    public void equipGrenadePistol()
    {
        grenadePistolEquipped = true;
        grenadePistol.gameObject.SetActive(true);
        grenadePistol.playCockingNoise();
    }

    public void unequipSword()
    {
        sword.SetActive(false);
        swordEquipped = false;
    }
    public void unequipGlock()
    {
        glock.disableMuzzleSmoke();
        glock.muzzleLight.SetActive(false);
        glock.gameObject.GetComponent<Animator>().enabled = false;
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
        assaultRifle.disableMuzzleSmoke();
        assaultRifle.muzzleLight.SetActive(false);
        assaultRifle.gameObject.SetActive(false);
        assaultrifleEquipped = false;
    }
    public void unequipRedDotRifle()
    {
        redDotRifle.disableMuzzleSmoke();
        redDotRifle.muzzleLight.SetActive(false);
        redDotRifle.gameObject.SetActive(false);
        redDotRifleEquipped = false;
    }
    public void unequipSingleShotRifle()
    {
        singleShotRifle.disableMuzzleSmoke();
        singleShotRifle.muzzleLight.SetActive(false);
        singleShotRifle.gameObject.SetActive(false);
        singleShotRifleEquipped = false;
    }
    public void unequipShotgun()
    {
        shotgun.disableMuzzleSmoke();
        shotgun.StopAllCoroutines();
        shotgun.muzzleLight.SetActive(false);
        shotgun.gameObject.SetActive(false);
        shotgunEquipped = false;
    }
    public void unequipDblBarrelShotgun()
    {
        dblBarrelShotgun.disableMuzzleSmoke();
        dblBarrelShotgun.StopAllCoroutines();
        dblBarrelShotgun.muzzleLight.SetActive(false);
        dblBarrelShotgun.gameObject.SetActive(false);
        dblBarrelShotgunEquipped = false;
    }
    public void unequipUzi()
    {
        uzi.disableMuzzleSmoke();
        uzi.StopAllCoroutines();
        uzi.muzzleLight.SetActive(false);
        uzi.gameObject.SetActive(false);
        uziEquipped = false;
    }
    public void unequipGrenadePistol()
    {
        grenadePistol.disableMuzzleSmoke();
        grenadePistol.StopAllCoroutines();
        grenadePistol.gameObject.SetActive(false);
        grenadePistolEquipped = false;
    }
}

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
    public PostProcessProfile postProcessProfile; // Assign the post-processing profile in the inspector
    private Vignette vignette;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public GameObject sliderFill;

    private float cooldownTimer = 0.0f;

    public bool swordEquipped;
    public bool grapplingGunEquipped;
    public bool glockEquipped;
    public GameObject grapplingGun;
    public GameObject sword;
    public Firearm glock;
    public TextMeshProUGUI ammoText;

    public AudioClip swordUnsheath;
    // Start is called before the first frame update
    void Start()
    {
        hpSlider.maxValue = maxHP;
        vignette = postProcessProfile.GetSetting<Vignette>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            swordEquipped = true;
            sword.SetActive(true);
            swordSwoosh.PlayOneShot(swordUnsheath);

            grapplingGun.SetActive(false);
            grapplingGunEquipped = false;
            glock.muzzleLight.SetActive(false);
            glock.gameObject.SetActive(false);
            glockEquipped = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            grapplingGunEquipped = true;
            grapplingGun.SetActive(true);

            sword.SetActive(false);
            swordEquipped = false;
            glock.muzzleLight.SetActive(false);
            glock.gameObject.SetActive(false);
            glockEquipped = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            glockEquipped = true;
            glock.gameObject.SetActive(true);
            glock.playCockingNoise();
            glock.anim.Play("GlockIdle");

            sword.SetActive(false);
            swordEquipped = false;
            grapplingGun.SetActive(false);
            grapplingGunEquipped = false;
        }

        hpSlider.value = HP;
        hpText.text = HP.ToString() + " %";

        if (glockEquipped)
        {
            ammoText.text = "Glock\n" + glock.ammoInMag.ToString() + " / " + glock.magCapacity.ToString();
        }
        else
        {
            ammoText.text = "";
        }
        if(HP < 1)
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
                if (glock.ammoInMag > 0 && !glock.reloading)
                {
                    StartCoroutine(glock.reload());
                }
                else if(glock.ammoInMag == 0 && !glock.reloading)
                {
                    StartCoroutine(glock.reloadOnEmpty());
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

        if(playerMovement.state == PlayerMovement.MovementState.air)
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
    }

    IEnumerator swordAttack()
    {
        slashOnCD = true;
        swordCollider.enabled = true;
        swordAnim.SetBool("isAttacking", true);
        playRandomSwordWoosh();
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
}

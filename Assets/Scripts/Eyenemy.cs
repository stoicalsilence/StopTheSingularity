using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyenemy : MonoBehaviour
{
    public int health = 3;
    public float knockbackForce = 5f;
    public float knockbackHeight = 1f; // Additional height for knockback
    public float jumpInterval = 2f;
    public float jumpForce = 5f;
    public float jumpHeight = 1.5f; // Additional height for jump
    public Transform player;
    public Renderer enemyRenderer;
    public Renderer enemyRendererRim1;
    public Renderer enemyRendererRim2;
    public Renderer enemyRendererRim3;
    public Renderer enemyRendererRim4;
    public Renderer enemyRendererScreen;
    public Material blackScreen;
    public Material Rim;
    public Material blueScreenOfDeath;
    public float flashDuration = 0.2f;
    public Color flashColor = Color.red;
    public float uprightDuration = 1f;
    public float bounceMultiplier = 2f;
    private Rigidbody rb;
    private bool isKnockedBack = false;
    private Color originalColor;

    public GameObject jumpParticles;
    public GameObject landParticles;
    public Transform jumpParticlePos;
    public GameObject collisionParticles;
    public GameObject explosionParticles;
    public GameObject angryFace;
    public GameObject happyFace;
    public GameObject sadFace;
    
    public AudioSource jumpSound;
    public AudioSource getHitSound;
    public AudioSource passiveSound;
    public AudioSource deadSound;
    public AudioClip[] getHitSounds;
    public AudioClip[] passiveSounds;
    public AudioClip[] dedSounds;
    private void Start()
    {
        blackScreen = enemyRendererScreen.material;
        rb = GetComponent<Rigidbody>();
        originalColor = enemyRenderer.material.color;
        InvokeRepeating("Jump", jumpInterval, jumpInterval);
        StartCoroutine(PlayPassiveSounds());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerAttack"))
        {
            TakeDamage();
            Knockback(collision.GetContact(0).point);
            FlashColor();
            FindObjectOfType<HitmarkerEffect>().ShowHitmarker();

            if (health > 0)
            {
                Vector3 collisionPoint = collision.GetContact(0).point;
                GameObject ded = Instantiate(collisionParticles, collisionPoint, Quaternion.identity);
                Destroy(ded, 5f);
            }
            else
            {
                Vector3 collisionPoint = collision.GetContact(0).point;
                GameObject oof = Instantiate(explosionParticles, collisionPoint, Quaternion.identity);
                Destroy(oof, 5f);
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("whatisGround"))
        {
            Vector3 collisionPoint = collision.GetContact(0).point;
            GameObject jp = Instantiate(landParticles, collisionPoint, Quaternion.identity);
            Destroy(jp, 4f);
        }

        if (collision.collider.CompareTag("Player"))
        {
            Vector3 direction = transform.position - collision.transform.position;
            direction.y = 0f;

            
        float jumpForce = Mathf.Sqrt(2f * Physics.gravity.magnitude * jumpHeight);

            jumpForce *= bounceMultiplier;

            rb.AddForce(direction.normalized * jumpForce, ForceMode.VelocityChange);
        }
    }

    private void TakeDamage()
    {
        health--;
        switchToSad();
        if (getHitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, getHitSounds.Length);
            AudioClip hitSound = getHitSounds[randomIndex];
            getHitSound.clip = hitSound;
            getHitSound.PlayOneShot(getHitSound.clip);
            ScreenShake.Shake(0.25f, 0.025f);
        }
        if (health <= 0)
        {
            if (dedSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, dedSounds.Length);
                AudioClip hitSound = dedSounds[randomIndex];
                deadSound.clip = hitSound;
                deadSound.PlayOneShot(deadSound.clip);
            }
            turnOffFace();
            resetColor();
            enemyRendererScreen.material = blueScreenOfDeath;
            RemoveTagsRecursively(transform);
            FindObjectOfType<KillText>().getReportedTo();
            ScreenShake.Shake(0.25f, 0.05f);
            Destroy(this);
        }
    }

    private void Knockback(Vector3 hitPoint)
    {
        isKnockedBack = true;

        Vector3 knockbackDirection = transform.position - hitPoint;
        knockbackDirection.y = 0f;
        knockbackDirection.Normalize();

        Vector3 knockbackForceVector = knockbackDirection * knockbackForce;
        knockbackForceVector.y += knockbackHeight; // Apply additional upward force

        rb.AddForce(knockbackForceVector, ForceMode.Impulse);

        //Invoke("ResetRotation", 1f);
    }

    private void ResetRotation()
    {
        StartCoroutine(SelfUprightCoroutine());
    }

    private IEnumerator SelfUprightCoroutine()
    {
        switchToHappy();
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position, Vector3.up);

        while (transform.rotation != targetRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f / uprightDuration * Time.deltaTime);
            yield return null;
        }

        isKnockedBack = false;
    }


    private void JumpTowardsPlayer()
    {
        if (!isKnockedBack)
        {
            jumpSound.PlayOneShot(jumpSound.clip);
            switchToAngry();
            Vector3 jumpDirection = player.position - transform.position;
            jumpDirection.Normalize();

            Vector3 jumpForceVector = jumpDirection * jumpForce;
            jumpForceVector.y += jumpHeight;

            rb.AddForce(jumpForceVector, ForceMode.Impulse);

            GameObject jp = Instantiate(jumpParticles, jumpParticlePos.position, Quaternion.identity);
            Destroy(jp, 4f);
        }
    }

    private void Jump()
    {
        StartCoroutine(SelfUprightCoroutine());
        Invoke("JumpTowardsPlayer", 1f);
    }

    private void FlashColor()
    {
        StartCoroutine(FlashColorCoroutine());
    }

    private IEnumerator FlashColorCoroutine()
    {
        enemyRenderer.material.color = flashColor;
        enemyRendererRim1.material.color = flashColor;
        enemyRendererRim2.material.color = flashColor;
        enemyRendererRim3.material.color = flashColor;
        enemyRendererRim4.material.color = flashColor;
        enemyRendererScreen.material = blueScreenOfDeath;

        yield return new WaitForSeconds(flashDuration);

        resetColor();
    }

    private void resetColor()
    {
        enemyRenderer.material.color = originalColor;
        enemyRendererRim1.material.color = originalColor;
        enemyRendererRim2.material.color = originalColor;
        enemyRendererRim3.material.color = originalColor;
        enemyRendererRim4.material.color = originalColor;
        enemyRendererScreen.material = blackScreen;
    }

    private IEnumerator PlayPassiveSounds()
    {
        while (true)
        {
            if (passiveSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, passiveSounds.Length);
                AudioClip sound = passiveSounds[randomIndex];
                passiveSound.PlayOneShot(sound);
            }

            float randomDelay = Random.Range(2f, 5f);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    private void switchToAngry()
    {
        sadFace.gameObject.SetActive(false);
        angryFace.gameObject.SetActive(true);
        happyFace.gameObject.SetActive(false);
    }

    private void switchToHappy()
    {
        sadFace.gameObject.SetActive(false);
        angryFace.gameObject.SetActive(false);
        happyFace.gameObject.SetActive(true);
    }

    private void switchToSad()
    {
        angryFace.gameObject.SetActive(false);
        happyFace.gameObject.SetActive(false);
        sadFace.gameObject.SetActive(true);
    }

    private void turnOffFace()
    {
        angryFace.gameObject.SetActive(false);
        happyFace.gameObject.SetActive(false);
        sadFace.gameObject.SetActive(false);
    }

    void RemoveTagsRecursively(Transform parent)
    {
        parent.gameObject.tag = "Box";

        foreach (Transform child in parent)
        {
            RemoveTagsRecursively(child);
        }
    }
}
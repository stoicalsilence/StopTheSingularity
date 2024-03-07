using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MrZaps : MonoBehaviour
{
    public bool triggered;
    public int currentHP;
    public int maxHP;
    public float speed;
    public Transform player;
    public Rigidbody rb;
    public float damping = 2;
    private float lastFootstepTime;
    public bool fireLaserBlocked, wanderingblocked, isDead;
    public NavMeshAgent agent;

    public Transform teslaTip;

    public AudioSource audioSource, BGM;
    public AudioClip stepSound, zap, shockaround, hurt, defeated, defeatReverb, chargeup, chargedown;
    public Animator animator;

    public ParticleSystem[] lasers;

    public GameObject shockwaveParticles, hitSparkles, defeatExplosion, defeatExplosion2;
    private bool isWandering;
    private float lastWanderTime;
    public float wanderInterval;
    private bool rotateTowardsPlayer;
    private bool isPlayerAlive;

    public bool active;
    public GameObject[] teslaTorusArray;
    public Material teslaTorusInactiveC;
    public Material teslaTorusActiveC;
    public Material targetColor;
    public AudioSource galvaSounds;
    public ParticleSystem galvaParticles;

    public Slider hpSlider;

    public bool isRamming;
    public AudioClip ramStartSound;

    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>().transform;
        animator.Play("Fall");
        teslaTorusInactiveC = teslaTorusArray[0].GetComponent<Renderer>().material;
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsWalking", false);

        hpSlider.maxValue = maxHP;

        hpSlider.gameObject.SetActive(false);
        getTriggered();
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            if (agent.velocity.magnitude > 0.5f)
            {
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
            isPlayerAlive = !player.GetComponent<Player>().dead;
            if (isWandering && !wanderingblocked)
            {
                lastWanderTime += Time.deltaTime;
                if (lastWanderTime >= wanderInterval)
                {
                    lastWanderTime = Random.Range(2, wanderInterval);
                    Vector3 dir = Calculate8Direction();
                    agent.SetDestination(dir);
                    agent.updateRotation = true;
                }
            }
            else
            {
                agent.ResetPath();
                agent.updateRotation = false;
                //if (rotateTowardsPlayer && isPlayerAlive)
                //{
                //    Vector3 directionToPlayer = player.position - transform.position;
                //    directionToPlayer.y = 0;

                //    Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
                //    targetRotation *= Quaternion.Euler(0f, 180, 0f);
                //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
                //}
            }
        }
    }

    public void toggleIsWandering()
    {
        isWandering = !isWandering;

        int r = 0;
        if (!isWandering)
        {
            r = Random.Range(2, 5);
        }
        else
        {
            r = Random.Range(7, 10);
        }
        
        Invoke("toggleIsWandering", r);
    }

    public void toggleActiveInactive()
    {
        active = !active;
        if (!active)
        {
            galvaParticles.enableEmission = false;
            hpSlider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        if (active)
        {
            galvaParticles.enableEmission = true;
            hpSlider.fillRect.GetComponent<Image>().color = Color.red;
        }

        Invoke("toggleActiveInactive", 7);
        Invoke("playChargeUpDownSound", 6.3f);
    }

    public void playChargeUpDownSound()
    {
        if (active)
        {
            audioSource.PlayOneShot(chargedown);
        }
        else
        {
            audioSource.PlayOneShot(chargeup);
        }
    }
    public void turnOnGalvaParticles()
    {
        galvaParticles.enableEmission = true;
    }
    public void FireLaser()
    {
        if (!fireLaserBlocked && active)
        {
            Vector3 directionToPlayer = (player.position - teslaTip.position).normalized;
            int inaccuracyRoll = Random.Range(1, 100);
            if (inaccuracyRoll < 50)
            {
                float r1 = Random.Range(-100, 100);
                float r2 = Random.Range(-100, 100);
                float r3 = Random.Range(-100, 100);
                directionToPlayer += new Vector3(directionToPlayer.x + r1, directionToPlayer.y + r2, directionToPlayer.z + r3);
            }
            else if (inaccuracyRoll > 50)
            {
                float r = Random.Range(-0.6f, 0.6f);
                directionToPlayer += new Vector3(directionToPlayer.x + r, directionToPlayer.y + r, directionToPlayer.z + r);
            }

            Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);

            foreach (var laserPrefab in lasers)
            {
                ParticleSystem newLaser = Instantiate(laserPrefab, teslaTip.position, Quaternion.identity);

                newLaser.transform.rotation = rotationToPlayer;
                newLaser.transform.position = teslaTip.position;
                ParticleSystem.ShapeModule s = newLaser.shape;
                float laserLength = 100;
                RaycastHit hit;
                if (Physics.Raycast(teslaTip.position, directionToPlayer, out hit, laserLength, LayerMask.NameToLayer("whatisground")))
                {
                    laserLength = hit.distance;
                }

                Vector3 newScale = new Vector3(s.scale.x, s.scale.y, laserLength);
                Vector3 newPos = new Vector3(0, 0, laserLength / 2);

                s.scale = newScale;
                s.position = newPos;

                newLaser.gameObject.tag = "Zap";

                Destroy(newLaser, 5f);
            }

            audioSource.PlayOneShot(zap);  
        }
        if (!isDead)
        {
            float r = Random.Range(0.3f, 1.4f);
            Invoke("FireLaser", r);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            ShockwaveAttack(collision);
        }

        if (collision.gameObject.tag == "PlayerAttack")
        {
            if (currentHP > 0)
            {
                if(active)
                {
                    ShockwaveAttack(collision);
                }
                if (!active)
                {
                    currentHP--;
                    hpSlider.value = currentHP;
                    audioSource.PlayOneShot(hurt);
                    Vector3 collisionPoint = collision.GetContact(0).point;
                    GameObject spark = Instantiate(hitSparkles, collisionPoint, Quaternion.identity);
                    Destroy(spark, 3f);
                    FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
                }
            }
            else
            {
                currentHP--;
                hpSlider.value = currentHP;
                audioSource.Stop();
                audioSource.PlayOneShot(defeated);
                hpSlider.gameObject.SetActive(false);
                triggered = false;
                animator.Play("Death");
                CancelInvoke("FireLaser");
                CancelInvoke("toggleIsWandering");
                fireLaserBlocked = true;
                isWandering = false;
                rotateTowardsPlayer = false;
                Invoke("explode", 1.90f);
                Vector3 collisionPoint = collision.GetContact(0).point;
                GameObject spark = Instantiate(hitSparkles, collisionPoint, Quaternion.identity);
                Destroy(spark, 3f);
                FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
                FindObjectOfType<KillText>().getReportedTo();
                if (BGM)
                {
                    BGM.Stop();
                    BGM.volume = 66;
                    BGM.PlayOneShot(defeatReverb);
                }

                FindObjectOfType<AfterPuteyBossCutscene>().StartCutscene();
            }
        }
    }

    public void ShockwaveAttack(Collision collision)
    {
        GameObject shockwave = Instantiate(shockwaveParticles, collision.GetContact(0).point, Quaternion.identity);
        Destroy(shockwave, 5f);
        Vector3 pushDirection = collision.transform.position - transform.position;
        pushDirection.y = Random.Range(0,4);

        pushDirection = pushDirection.normalized;
        float playerPushForce = 80;

        audioSource.PlayOneShot(shockaround);

        player.gameObject.GetComponent<Rigidbody>().AddForce(pushDirection * playerPushForce, ForceMode.Impulse);

        FindObjectOfType<Player>().takeDamage(2);
    }

    private Vector3 Calculate8Direction()
    {
        int random = Random.Range(0, 8);
        float distance = Random.Range(5f, 8.0f);

        Vector3 newPosition = transform.position;

        switch (random)
        {
            case 0:
                // Move forward
                newPosition += transform.forward * distance;
                break;
            case 1:
                // Move forward-left
                newPosition += (transform.forward - transform.right).normalized * distance;
                break;
            case 2:
                // Move left
                newPosition -= transform.right * distance;
                break;
            case 3:
                // Move back-left
                newPosition -= (transform.forward + transform.right).normalized * distance;
                break;
            case 4:
                // Move backward
                newPosition -= transform.forward * distance;
                break;
            case 5:
                // Move back-right
                newPosition -= (-transform.forward + transform.right).normalized * distance;
                break;
            case 6:
                // Move right
                newPosition += transform.right * distance;
                break;
            case 7:
                // Move forward-right
                newPosition += (transform.forward + transform.right).normalized * distance;
                break;
        }

        return newPosition;
    }

    void explode()
    {
        Instantiate(defeatExplosion, transform.position, Quaternion.identity);
        Instantiate(defeatExplosion2, transform.position, Quaternion.identity);
        ScreenShake.Shake(0.75f, 0.4f);
        Destroy(this.gameObject);
    }

    IEnumerator UpdateColor()
    {
        while (true)
        {
            targetColor = active ? teslaTorusActiveC : teslaTorusInactiveC;
            float targetVolume = active ? 1f : 0;

            for (int i = 0; i < teslaTorusArray.Length; i++)
            {
                Renderer renderer = teslaTorusArray[i].GetComponent<Renderer>();
                Material currentMaterial = renderer.material;

                // Lerp between material properties
                Material lerpedMaterial = new Material(currentMaterial);
                lerpedMaterial.color = Color.Lerp(currentMaterial.color, targetColor.color, Time.deltaTime);
                lerpedMaterial.mainTexture = targetColor.mainTexture;  // Example: Lerp main texture if applicable

                renderer.material = lerpedMaterial;
            }

            galvaSounds.volume = Mathf.Lerp(galvaSounds.volume, targetVolume, Time.deltaTime);

            yield return null;
        }
    }

    private void ramAttack()
    {
        isWandering = false;
        audioSource.PlayOneShot(ramStartSound);
        animator.Play("Ramming");
        Invoke("setrammingtrue", 1.2f);
        Invoke("setrammingfalse", 2.4f);
    }
    void setrammingtrue()
    {
        isRamming = true;
    }
    void setrammingfalse()
    {
        isRamming = false;
    }

    public void getTriggered()
    {
        triggered = true;
        Invoke("FireLaser", 5);
        Invoke("toggleIsWandering", 5);
        hpSlider.maxValue = maxHP;
        StartCoroutine(UpdateColor());
        hpSlider.gameObject.SetActive(true);

        playChargeUpDownSound();
        toggleActiveInactive();
        Invoke("screenshake", 0.8f);
        Invoke("startBGM", 1.5f);
    }

    void screenshake()
    {
        ScreenShake.Shake(0.75f, 0.4f);
    }
    public void startBGM()
    {
        BGM.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LordPutey : MonoBehaviour
{
    bool rotateTowardsPlayer;
    Transform player;
    Rigidbody rb;
    public float upwardPushForce = 440;
    public GameObject orbiterPrefab;
    public List<GameObject> orbiters;
    public Transform orbitPoint;

    public Slider hpSlider;
    public int HP;
    bool ded;
    public GameObject shield;

    public AudioSource audioSource;
    public GameObject hitSparks;
    public AudioClip floatSound, hitsound, shieldhitSound, shockWave;

    public GameObject angyface, sadfaec;

    public AudioSource BGM;
    public AudioClip defeatReverb;
    // Start is called before the first frame update
    void Start()
    {
        BGM = FindObjectOfType<FinalBossAssorter>().BGM;
        hpSlider = FindObjectOfType<FinalBossAssorter>().lordPuteySlider;
        hpSlider.maxValue = HP;
        hpSlider.value = HP;
        hpSlider.gameObject.SetActive(true);
        hpSlider.fillRect.GetComponent<Image>().color = Color.yellow;
        player = FindObjectOfType<Player>().transform;
        rb = GetComponent<Rigidbody>();
        rotateTowardsPlayer = true;
        TogglePushForce();

        InvokeRepeating("SpawnOrbiter", 3, 7);
    }

    // Update is called once per frame
    void Update()
    {
        if(orbiters.Count > 0)
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.gray;
            shield.gameObject.SetActive(true);
        }
        else
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.yellow;
            shield.gameObject.SetActive(false);
        }
        rb.AddForce(Vector3.up * upwardPushForce * Time.deltaTime);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if(distanceToPlayer > 10 && !ded)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            transform.position += directionToPlayer * 5 * Time.deltaTime;
        }

        if (rotateTowardsPlayer)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnOrbiter();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAttack" && !ded)
        {
            if (orbiters.Count > 0)
            {
                audioSource.PlayOneShot(shieldhitSound);
            }
            else
            {
                audioSource.PlayOneShot(hitsound);
                HP--;
                FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
                Vector3 collisionPoint = collision.GetContact(0).point;
                GameObject spark = Instantiate(hitSparks, collisionPoint, Quaternion.identity);
                Destroy(spark, 3f);
                hpSlider.value = HP;
            }

            if(HP < 1 && !ded)
            {
                FindObjectOfType<KillText>().getReportedTo();
                hpSlider.gameObject.SetActive(false);
                FindObjectOfType<AfterPuteyBossCutscene>().StartCutscene();
                ded = true;
                upwardPushForce = 0;
                angyface.SetActive(false);
                sadfaec.SetActive(true); BGM.Stop();
                BGM.volume = 66;
                BGM.PlayOneShot(defeatReverb);
            }
        }
    }

    public void playShockwave()
    {
        audioSource.PlayOneShot(shockWave);
    }
    void TogglePushForce()
    {
        if (!ded)
        {
            rb.velocity = Vector3.zero;
            if (upwardPushForce == 480 * 6)
            {
                upwardPushForce = 560 * 6;
            }
            else
            {
                upwardPushForce = 480 * 6;
            }

            Invoke("TogglePushForce", 1.5f);
        }
    }

    void SpawnOrbiter()
    {
        if (orbiters.Count < 5 && !ded)
        {
            GameObject newOrbiter = Instantiate(orbiterPrefab, transform.position, Quaternion.identity);
            // newOrbiter.transform.parent = transform;
            //newOrbiter.GetComponent<Orbiter>().orbitCenter = orbitPoint.position;
            orbiters.Add(newOrbiter);
        }
    }
}

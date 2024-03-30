using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbiter : MonoBehaviour
{
    public int hp = 3;
    public Vector3 orbitCenter;

    public GameObject explosion;
    public Transform player;
    public GameObject bulletPrefab;
    public AudioSource audioSource;
    public AudioClip pew;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<Player>().transform;
        audioSource.Play();
        InvokeRepeating("Shoot", 2.5f, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        orbitCenter = FindObjectOfType<LordPutey>().orbitPoint.position;
        Vector3 relativePos = (orbitCenter) - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        Quaternion current = transform.localRotation;

        transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime);
        transform.Translate(0, 0, 3 * Time.deltaTime);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerAttack")
        {
            hp--;
            FindObjectOfType<HitmarkerEffect>().ShowHitmarker();
            if(hp < 1)
            {
                Defeat();
            }
        }
    }

    void Defeat()
    {
        FindObjectOfType<KillText>().getReportedTo();
        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(expl, 5f);
        FindObjectOfType<LordPutey>().orbiters.Remove(this.gameObject);
        if(FindObjectOfType<LordPutey>().orbiters.Count == 0)
        {
            FindObjectOfType<LordPutey>().playShockwave();
        }
        Destroy(this.gameObject);
    }

    void Shoot()
    {
        // Check if there's a clear line of sight to the player
        Vector3 directionToPlayer = (player.position + new Vector3(0, -0.5f, 0)) - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit))
        {
            if (hit.transform.CompareTag("Player"))
            {
                // There is a clear line of sight to the player, proceed with shooting
                audioSource.PlayOneShot(pew);

                GameObject bulletObject = Instantiate(bulletPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                Destroy(bulletObject, 3f);
                Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();
                bulletRigidbody.velocity = directionToPlayer.normalized * 20;
            }
        }
        else
        {
            // Raycast didn't hit anything, assuming there's a clear line of sight
            audioSource.PlayOneShot(pew);

            GameObject bulletObject = Instantiate(bulletPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            Destroy(bulletObject, 3f);
            Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = directionToPlayer.normalized * 20;
        }
    }
}

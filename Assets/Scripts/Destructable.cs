using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource audioSource;
    public AudioClip[] hitClips;
    public AudioClip[] destroyClips;

    public GameObject hitParticles;
    public GameObject destroyParticles;
    public bool destroyableBySlide;
    public int hp;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnDestroyParticles()
    {
        GameObject ded = Instantiate(destroyParticles, transform.position, Quaternion.identity);
        Destroy(ded, 5f);
    }

    public void explode()
    {
        int randomIndex = Random.Range(0, destroyClips.Length);
        AudioClip sound = destroyClips[randomIndex];
        audioSource.gameObject.transform.SetParent(null);
        audioSource.PlayOneShot(sound);
        Destroy(audioSource.gameObject, 5f);
        if (gameObject.GetComponent<MeshDestroy>())
        {
            gameObject.GetComponent<MeshDestroy>().DestroyMesh();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && FindObjectOfType<PlayerMovement>().isCrouching && destroyableBySlide && FindObjectOfType<PlayerMovement>().slideSpeed > 5)
        {
            Vector3 collisionPoint = collision.GetContact(0).point;
            GameObject ded = Instantiate(destroyParticles, collisionPoint, Quaternion.identity);
            Destroy(ded, 5f);
            explode();
        }

        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            hp--;

            if (hp <= 0)
            {
                Vector3 collisionPoint = collision.GetContact(0).point;
                GameObject ded = Instantiate(destroyParticles, collisionPoint, Quaternion.identity);
                Destroy(ded, 5f);
                explode();
            }
            else
            {
                int randomIndex = Random.Range(0, hitClips.Length);
                AudioClip sound = hitClips[randomIndex];
                audioSource.PlayOneShot(sound);
                Vector3 collisionPoint = collision.GetContact(0).point;
                GameObject oof = Instantiate(destroyParticles, collisionPoint, Quaternion.identity);
                Destroy(oof, 5f);
            }
        }
    }
}

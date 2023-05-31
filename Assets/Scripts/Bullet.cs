using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletHitFx;

    public AudioSource impactSound;
    public string ignoreTag;
    public AudioClip[] impacts;
    // Start is called before the first frame update
    private void Start()
    {
        base.Invoke("DestroySelf", 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {   if(other.gameObject.tag == "EnemyBody")
        {
            int randomIndex = Random.Range(0, impacts.Length);
            AudioClip sound = impacts[randomIndex];
            impactSound.gameObject.transform.SetParent(null);
            impactSound.PlayOneShot(sound);
            Destroy(impactSound.gameObject, 5f);
            GameObject parts = Instantiate(bulletHitFx, transform.position, Quaternion.identity);
            Destroy(parts, 5f);
            DestroySelf();
        }
        if (other.gameObject.CompareTag(ignoreTag))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), other.collider);
        }
        else
        {
            int randomIndex = Random.Range(0, impacts.Length);
            AudioClip sound = impacts[randomIndex];
            impactSound.gameObject.transform.SetParent(null);
            impactSound.PlayOneShot(sound);
        Destroy(impactSound.gameObject, 5f);
            GameObject parts = Instantiate(bulletHitFx, transform.position, Quaternion.identity);
            Destroy(parts, 5f);
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        Object.Destroy(base.gameObject);
    }
}

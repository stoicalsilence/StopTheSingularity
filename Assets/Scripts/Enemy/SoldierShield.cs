using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierShield : MonoBehaviour
{
    public GameObject explosionParticles, hitParticles, cracks;
    public int health;
    public AudioSource audioS;
    public AudioClip hit, broken;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAttack")
        {
            Vector3 collisionPoint = collision.GetContact(0).point;
            if (health > 1)
            {
                health--;
                GameObject part = Instantiate(hitParticles, collisionPoint, Quaternion.identity);
                Destroy(part, 4);
                audioS.PlayOneShot(hit);

                if(health <4)
                {
                    cracks.gameObject.SetActive(true);
                }
            }
            else
            {
                audioS.gameObject.transform.SetParent(null);
                audioS.PlayOneShot(broken);
                Destroy(audioS, 5f);
                GameObject part = Instantiate(explosionParticles, transform.position, Quaternion.identity);
                Destroy(part, 4);
                Destroy(this.gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jug : MonoBehaviour
{
    public GameObject particles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        GameObject fart = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(fart, 5f);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PlayerAttack")
        {
            Explode();
        }
    }
}

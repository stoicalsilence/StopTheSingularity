using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool attacking;

    public AudioSource audioSource;
    public AudioClip stepSound, zap, shockaround;
    public Animator animator;

    public ParticleSystem[] lasers;

    public GameObject shockwaveParticles, laserZapParticles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireLaser()
    {
        Vector3 defaultScale = lasers[0].shape.scale;
        float laserLength = 100;

        RaycastHit hit;
        UnityEngine.Debug.DrawLine(transform.position, player.position + transform.right * 30, Color.black, 10);
        if (Physics.Raycast(transform.position, player.position, out hit, laserLength, LayerMask.NameToLayer("whatisground")))
        {
            laserLength = hit.distance;
        }

        Vector3 newScale = new Vector3(defaultScale.x, defaultScale.y, laserLength);
        Vector3 newPos = new Vector3(0, 0, laserLength / 2);

        foreach (var l in lasers)
        {
            ParticleSystem.ShapeModule s = l.shape;
            s.scale = newScale;
            s.position = newPos;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            ShockwaveAttack(collision);
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

        player.gameObject.GetComponent<Rigidbody>().AddForce(pushDirection * playerPushForce, ForceMode.Impulse);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Getty : MonoBehaviour
{
    public NavMeshAgent agent;
    public KjoyunBodyPart Legs;
    public KjoyunBodyPart LeftArm;
    public KjoyunBodyPart RightArm;
    public KjoyunBodyPart Body;
    //When KyojunbodyPart is destroyed, i cant explode and hide it... well unless i hide an array of objects then i can. otherwise just add the destroyed particles and whatever
    public GameObject legDestroyedParticles;
    public GameObject leftArmDestroyedParticles;
    public GameObject rightArmDestroyedParticles;
    public GameObject bodyDestroyedParticles;

    public Transform player;
    public Transform gunTip;
    public Transform ejectionPoint;

    public Animator animator;

    bool ischasing, attacking, rotateTowardsPlayer;

    public AudioSource gunAudioSource;
    public AudioSource missileLauncherAudioSource;

    public GameObject bullet, orangeLight, bulletCasing;
    public GameObject missile;
    public ParticleSystem muzzleFlare, muzzleSmoke;

    public float bulletInaccuracy;
    bool dead = false;
    public GameObject slidersContainer;
    public AudioSource generalAudiosource;

    bool canttogglechasing;

    public AnimationClip Aim, Shot, Unaim;

    public AudioClip gunshot1, gunshot2, mechmove, mechmove2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator RangedAttack()
    {
        attacking = true;
        rotateTowardsPlayer = true;

        animator.Play(Aim.name);
        bool rmove = Random.Range(0, 100) < 50;
        AudioClip move = rmove ? mechmove : mechmove2;
        gunAudioSource.PlayOneShot(move);
        int shotsFired = 0;
        yield return new WaitForSeconds(Aim.length);
        while (shotsFired < 10)
        {
            if (dead)
            {
                rotateTowardsPlayer = false;
                yield break;
            }
            bool r = Random.Range(0, 100) < 50;
            AudioClip gunshot = r ? gunshot1 : gunshot2;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            gunAudioSource.PlayOneShot(gunshot);
            
                animator.Play(Shot.name);
            
            
            if (LeftArm.currentHP < 1)
            {
                animator.Play(Unaim.name);
                yield return new WaitForSeconds(Unaim.length);
                attacking = false;
                rotateTowardsPlayer = false;
                yield break;
            }

            shotsFired++;
            muzzleFlare.Play();
            StartCoroutine(bulletLight());
            Vector3 directionToPlayer = player.position - gunTip.position;
            directionToPlayer = ApplyBulletInaccuracy(directionToPlayer);
            GameObject bulletObject = Instantiate(bullet, gunTip.position, Quaternion.identity);
            Destroy(bulletObject, 3f);
            Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = directionToPlayer.normalized * 19;

            GameObject casing = Instantiate(bulletCasing, ejectionPoint.position, Quaternion.identity);
            casing.transform.eulerAngles = new Vector3(
                    casing.transform.eulerAngles.x + 90,
                    casing.transform.eulerAngles.y,
                    casing.transform.eulerAngles.z);

            float forceMagnitude = 5f;
            float torqueMagnitude = 2f;
            casing.GetComponent<Rigidbody>().AddForce(-transform.right * forceMagnitude, ForceMode.Impulse);
            casing.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * torqueMagnitude, ForceMode.Impulse);

            yield return new WaitForSeconds(Shot.length);
        }
        animator.Play(Unaim.name);
        muzzleSmoke.Play();
        yield return new WaitForSeconds(Unaim.length);
        attacking = false;
        rotateTowardsPlayer = false;
        canttogglechasing = true;
        Invoke("toggleIsChasingImmunity", 5f);
        ischasing = true;
    }

    private Vector3 ApplyBulletInaccuracy(Vector3 direction)
    {
        float horizontalInaccuracyAngle = Random.Range(-bulletInaccuracy, bulletInaccuracy);
        float verticalInaccuracyAngle = Random.Range(-bulletInaccuracy, bulletInaccuracy);
        Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalInaccuracyAngle, Vector3.up);
        direction = horizontalRotation * direction;
        Quaternion verticalRotation = Quaternion.AngleAxis(verticalInaccuracyAngle, Vector3.right);
        direction = verticalRotation * direction;

        return direction;
    }
    public IEnumerator bulletLight()
    {
        orangeLight.SetActive(true);
        yield return new WaitForSeconds(0.16f);
        orangeLight.SetActive(false);
    }
}

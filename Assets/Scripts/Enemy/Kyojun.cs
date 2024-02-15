using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kyojun : MonoBehaviour
{
    public NavMeshAgent agent;
    public KjoyunBodyPart Legs;
    public KjoyunBodyPart LeftArm;
    public KjoyunBodyPart RightArm;
    public KjoyunBodyPart Body;

    public GameObject legDestroyedParticles;
    public GameObject leftArmDestroyedParticles;
    public GameObject rightArmDestroyedParticles;

    public GameObject leftArmModelToDestroy;
    public GameObject rightArmModelToDestroy;
    public Transform player;

    public Animator animator;
    public AnimationClip Aim, Shot, Unaim;
    bool ischasing, rangeAttacking, rotateTowardsPlayer;
    public AudioSource gunAudioSource;
    public AudioClip gunshot1, gunshot2;

    public IEnumerator RangedAttack()
    {
        rangeAttacking = true;
        rotateTowardsPlayer = true;

        animator.Play(Aim.name);
        int shotsFired = 0;
        yield return new WaitForSeconds(Aim.length);
        while (shotsFired < 10)
        {    
            animator.Play(Shot.name);
            bool r = Random.Range(0, 100) <50;
            AudioClip gunshot = r ? gunshot1 : gunshot2;
            gunAudioSource.PlayOneShot(gunshot);
            shotsFired++;
            yield return new WaitForSeconds(Shot.length);
        }
        animator.Play(Unaim.name);
        yield return new WaitForSeconds(Unaim.length);
        rangeAttacking = false;
        rotateTowardsPlayer = false;
        //TODO: While rangeAttacking, change z rotation of gun arm to look at player
    }
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>().transform;
        agent.updateRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(RangedAttack());
        }
        if (rotateTowardsPlayer)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
            targetRotation *= Quaternion.Euler(0f, 180, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);
        }
        if (agent.velocity.magnitude > 0.5f)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
        if (ischasing)
        {
            agent.SetDestination(player.position);
        }
        if(Legs.currentHP < 1)
        {
            legDestroyedParticles.gameObject.SetActive(true);
        }
        if (LeftArm.currentHP < 1)
        {
            leftArmDestroyedParticles.gameObject.SetActive(true);
            leftArmModelToDestroy.gameObject.SetActive(false);
        }
        if (RightArm.currentHP < 1)
        {
            rightArmDestroyedParticles.gameObject.SetActive(true);
            rightArmModelToDestroy.gameObject.SetActive(false);
        }
    }

    void toggleIsChasing()
    {
        if (ischasing)
        {
            agent.ResetPath();
        }
        ischasing = !ischasing;
    }
}

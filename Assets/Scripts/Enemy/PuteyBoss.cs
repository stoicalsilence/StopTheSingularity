using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PuteyBoss : MonoBehaviour
{
    public bool triggered;
    public float speed;
    public Transform player;
    public NavMeshAgent agent;
    public Rigidbody rb;
    public float damping = 2;
    public bool playerVeryClose;
    public bool stunned;
    public bool attacking;


    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //InvokeRepeating("PlayPassiveSounds", 0.1f, 5);
        player = FindObjectOfType<Player>().transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.updateRotation = false;
    }
   
    void Update()
    {
        if (triggered)
        {
            if (!playerVeryClose && !stunned)
            {
                RunAndTurnTowardsPlayer();
            }
        }
    }

    private void RunAndTurnTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > 4f)
        {
            animator.SetBool("Walking", true);
            animator.SetBool("Idle", false);
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(-directionToPlayer, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);

            agent.SetDestination(player.position);
        }
        else
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Idle", true);
            if (!playerVeryClose) { playerVeryClose = true; Invoke("resetPlayerVeryClose", 1); }
        }
    }

    void resetPlayerVeryClose()
    {
        playerVeryClose = false;
    }
}

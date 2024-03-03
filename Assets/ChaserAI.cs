using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.XR.OpenXR.Features.Interactions.DPadInteraction;

public class ChaserAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange;
    public bool damaged;
    public int health = 10;
    public int timeoutAttackedColor = 1;
    public bool dead;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        if (playerInSightRange) ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (damaged || dead) return;

        print("Chasing player");
        agent.SetDestination(player.position);
    }

    public void TakeDamage(int damage)
    {
        damaged = true;
        health -= damage;
            MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (health <= 0)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            Invoke(nameof(DestroyChaser), 1.5f);
            dead = true;
            agent.isStopped = true;
            renderer.material.color = Color.black;
        }
        else {
            renderer.material.color = Color.red;
            Invoke(nameof(Resume), timeoutAttackedColor);
        }
    }

    private void DestroyChaser()
    {
        Destroy(gameObject);
    }

    private void Resume() {
        damaged = false;
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.color = Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

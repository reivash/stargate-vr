using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.XR.OpenXR.Features.Interactions.DPadInteraction;

public class ChaserAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public MeshRenderer meshRenderer;
    public Rigidbody rigidBody;
    AudioSource audioSource;
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
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;   
    }

    private void Update()
    {
        if (!agent.enabled) return;
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        if (playerInSightRange) ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (damaged || dead) return;

        print("Chasing player");
        agent.SetDestination(player.position);
    }

    public void TakeDamage(int damage, Vector3 hitDirection)
    {
        if (!agent.enabled || (agent.isOnNavMesh && agent.isStopped)) return;
        rigidBody.isKinematic = false;
        agent.enabled = false;
        rigidBody.AddForce(hitDirection.normalized * damage/10, ForceMode.Impulse);

        damaged = true;
        health -= damage;
        if (health <= 0)
        {
            audioSource.Play();
            Invoke(nameof(DestroyChaser), 1.5f);
            dead = true;
           if (agent.isOnNavMesh) agent.isStopped = true;
            meshRenderer.material.color = Color.black;
        }
        else {
            meshRenderer.material.color = Color.red;
            Invoke(nameof(Resume), timeoutAttackedColor);
        }
    }

    private void DestroyChaser()
    {
        Destroy(gameObject);
    }

    private void Resume() {
        agent.enabled = true;
        rigidBody.isKinematic = false;
        damaged = false;
        meshRenderer.material.color = Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

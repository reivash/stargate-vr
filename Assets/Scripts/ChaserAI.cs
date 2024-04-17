using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.XR.OpenXR.Features.Interactions.DPadInteraction;

public class ChaserAI : MonoBehaviour {


    public NavMeshAgent agent;
    private Rigidbody rigidBody;
    AudioSource audioSource;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject orcGameObject;
    private Animator animator;
    public RuntimeAnimatorController idleController;
    public RuntimeAnimatorController walkController;
    public RuntimeAnimatorController attackController;
    public RuntimeAnimatorController dieController;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float sightRange, attackRange;
    private bool playerInSightRange;
    private bool playerInAttackRange;
    public bool damaged;
    public int health = 10;
    public int timeoutAttackedColor = 1;
    public bool dead;
    public NavMeshSurface surface;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();
        animator = orcGameObject.GetComponent<Animator>();
        surface.BuildNavMesh();
    }

    private void Update() {
        if (!agent.enabled) return;
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (playerInAttackRange) {
            animator.runtimeAnimatorController = attackController;
        } else
        if (playerInSightRange) {
            ChasePlayer();
        } else {
            animator.runtimeAnimatorController = idleController;
        }
    }

    private void ChasePlayer() {
        if (damaged || dead) return;
        if (agent.isOnNavMesh)
            agent.SetDestination(player.position);
        animator.runtimeAnimatorController = walkController;
    }

    public void TakeDamage(int damage, Vector3 hitDirection) {
        if (!agent.enabled || (agent.isOnNavMesh && agent.isStopped)) return;
        //rigidBody.AddForce(hitDirection.normalized * damage / 10, ForceMode.Impulse);

        damaged = true;
        health -= damage;
        if (health <= 0) {
            audioSource.Play();
            Invoke(nameof(DestroyChaser), 1.5f);
            dead = true;
            //if (agent.isOnNavMesh) agent.isStopped = true;
            animator.runtimeAnimatorController = dieController;
        } else {
            Invoke(nameof(Resume), timeoutAttackedColor);
        }
    }

    private void DestroyChaser() {
        Destroy(gameObject);
    }

    private void Resume() {
        //agent.enabled = true;
        damaged = false;
        animator.runtimeAnimatorController = idleController;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

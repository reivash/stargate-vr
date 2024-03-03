using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        if (damaged) return;

        print("Chasing player");
        agent.SetDestination(player.position);
    }

    public void TakeDamage(int damage) {
        damaged = true;
        health -= damage;
        if (health <= 0) {
            Destroy(gameObject);
        }
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.color = Color.red;
        Invoke(nameof(Resume), timeoutAttackedColor);
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

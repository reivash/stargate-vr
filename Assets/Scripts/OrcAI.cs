using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.XR.OpenXR.Features.Interactions.DPadInteraction;

public class OrcAI : MonoBehaviour {

    public NavMeshAgent agent;
    private Rigidbody rigidBody;
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
    public bool attacking;
    public int health = 10;
    public int timeoutAttackedColor = 1;
    public float attackSpeed = 1.5f;
    public bool dead;
    public NavMeshSurface surface;
    public float orcDamage = 1;

    private AudioSource audioSource;
    private AudioClip orcAttackAudioClip;
    private AudioClip orcDamagedAudioClip;
    private AudioClip orcDieAudioClip;


    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        orcAttackAudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/orc-attack.mp3");
        orcDamagedAudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/orc-damaged.mp3");
        orcDieAudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/orc-die.mp3");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        animator = orcGameObject.GetComponent<Animator>();
        surface.BuildNavMesh();
    }

    private void Update() {
        if (!agent.enabled || attacking) return;
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (playerInAttackRange) {
            attacking = true;
            Invoke(nameof(ReadyToAttack), attackSpeed);
            animator.runtimeAnimatorController = attackController;
            if (!audioSource.isPlaying) audioSource.PlayOneShot(orcAttackAudioClip);
            PlayerHealthController playerHealthController = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<PlayerHealthController>();
            playerHealthController.TakeDamage(orcDamage);
        } else
        if (playerInSightRange) {
            ChasePlayer();
        } else {
            animator.runtimeAnimatorController = idleController;
        }
    }
    private void ReadyToAttack() {
        attacking = false;
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
            if (!audioSource.isPlaying) audioSource.PlayOneShot(orcDieAudioClip);
            Invoke(nameof(DestroyChaser), 1.5f);
            dead = true;
            //if (agent.isOnNavMesh) agent.isStopped = true;
            animator.runtimeAnimatorController = dieController;
        } else {
            Invoke(nameof(Resume), timeoutAttackedColor);
            if (!audioSource.isPlaying) audioSource.PlayOneShot(orcDieAudioClip);
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

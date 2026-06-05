using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class ShadowEnemy : MonoBehaviour
{
    // 1. SETTINGS (Simplificados al máximo)
    [Header("Basics")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float retreatDistance = 8f;

    // 2. REFERENCIAS INTERNAS
    private Transform playerTarget;
    private NavMeshAgent agent;
    private PlayerSanity playerSanity;
    private CinemachineImpulseSource impulse;

    // 3. ESTADO
    private bool isRetreating;
    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        impulse = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null) playerTarget = player.transform;
            else return; 
        }

        if (isRetreating)
        {
            return;
        }

        // Perseguir al jugador
        agent.isStopped = false;
        agent.SetDestination(playerTarget.position);

        if (Time.time >= nextAttackTime && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        agent.isStopped = true;

        
        if (playerSanity == null) playerSanity = playerTarget.GetComponent<PlayerSanity>();
        playerSanity.TakeDamage(damage);
        if (impulse != null) impulse.GenerateImpulse();

        EnemyAudio enemyAudio = GetComponent<EnemyAudio>();

        if (enemyAudio != null)
        {
            enemyAudio.PlayAttackSound();
        }

        // Huida
        isRetreating = true;
        Vector3 retreatDir = (transform.position - playerTarget.position).normalized;
        Vector3 retreatPoint = transform.position + retreatDir * retreatDistance;
        
        agent.isStopped = false;
        agent.SetDestination(retreatPoint);

        yield return new WaitForSeconds(attackCooldown);

       
        nextAttackTime = Time.time;
        isRetreating = false;
    }
}
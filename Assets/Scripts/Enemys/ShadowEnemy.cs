using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class ShadowEnemy : MonoBehaviour
{
    enum State { Chasing, Retreating }
    private State currentState = State.Chasing;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float baseCooldown = 4f; 
    [SerializeField] private float minCooldown = 1f;   
    [SerializeField] private float escalationRate = 0.05f; 
    [SerializeField] private float retreatDistance = 5f; 

    private Transform target;
    private NavMeshAgent agent;
    private PlayerSanity playerSanity;
    private CinemachineImpulseSource impulse;
    
    private float currentCooldown;
    private float timeInRoom;
    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        impulse = GetComponent<CinemachineImpulseSource>();
        
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null) target = player.transform;
    }

    private void OnEnable()
    {
        timeInRoom = 0f; 
        currentCooldown = baseCooldown;
        currentState = State.Chasing;
    }

    private void Update()
    {
        if (target == null) return;

        // Más rápido con el tiempo en la habitación
        timeInRoom += Time.deltaTime;
        currentCooldown = Mathf.Max(minCooldown, baseCooldown - (timeInRoom * escalationRate));

        float distance = Vector3.Distance(transform.position, target.position);

        if (currentState == State.Chasing)
        {
            if (distance <= detectionRadius)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);

                if (distance <= agent.stoppingDistance && Time.time >= nextAttackTime)
                {
                    ExecuteAttack();
                }
            }
            else
            {
                agent.isStopped = true;
            }
        }
    }

    private void ExecuteAttack()
    {
        if (playerSanity == null) playerSanity = target.GetComponent<PlayerSanity>();
        
        playerSanity.TakeDamage(damageAmount);
        if (impulse != null) impulse.GenerateImpulse();

        StartCoroutine(RetreatRoutine());
    }

    private IEnumerator RetreatRoutine()
    {
        currentState = State.Retreating;
        
        // Se aleja del jugador en dirección opuesta
        Vector3 retreatDir = (transform.position - target.position).normalized;
        Vector3 retreatPoint = transform.position + retreatDir * retreatDistance;
        
        agent.SetDestination(retreatPoint);

        yield return new WaitForSeconds(currentCooldown);
        
        nextAttackTime = Time.time;
        currentState = State.Chasing;
    }
}
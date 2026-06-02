using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private string playerTag = "Player";

    [Header("Movement")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float turnSpeed = 5f;

    private Transform target;
    private Animator animator;
    private bool isPlayerInAttackRange = false; 
    private void Awake()
    {
        animator = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null) target = player.transform;
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= detectionRadius)
        {
            ChaseTarget();
        }
        else
        {
            Idle();
        }
    }

    private void ChaseTarget()
    {
        // 1. Rotación: Siempre mira al jugador, esté lejos o cerca atacando
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; 

        if (direction.sqrMagnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // 2. Movimiento: SOLO avanza si el escudo invisible NO está tocando al jugador
        if (!isPlayerInAttackRange)
        {
            transform.position += transform.forward * chaseSpeed * Time.deltaTime;
            if (animator != null) animator.SetBool("isChasing", true);
        }
        else
        {
            // Si ya ha llegado a la distancia de ataque, se para y apaga la animación de correr
            if (animator != null) animator.SetBool("isChasing", false);
        }
    }

    private void Idle()
    {
        if (animator != null) animator.SetBool("isChasing", false);
        isPlayerInAttackRange = false; 
    }

    // --- DETECCIÓN POR COLISIÓN PARA EL FRENAZO ---
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInAttackRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInAttackRange = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
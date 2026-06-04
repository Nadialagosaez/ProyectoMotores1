using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class DollEnemy : MonoBehaviour
{
    enum State { Waiting, Retreating }
    private State currentState = State.Waiting;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject dollVisuals;
    [SerializeField] private float damageAmount = 15f;
    [SerializeField] private float baseCooldown = 5f;
    [SerializeField] private float minCooldown = 1.5f;
    [SerializeField] private float escalationRate = 0.08f;
    [SerializeField] private float retreatDistance = 7f;
    [SerializeField] private float lookDurationRequired = 2f; 

    private Transform target;
    private Camera playerCam;
    private NavMeshAgent agent;
    private PlayerSanity playerSanity;
    private CinemachineImpulseSource impulse;
    private Animator animator;
    

    private float currentCooldown;
    private float timeInRoom;
    private float lookTimer; 
    private bool canAttack = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        impulse = GetComponent<CinemachineImpulseSource>();
        playerCam = Camera.main;
        
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null) target = player.transform;
    }

    private void OnEnable()
    {
        timeInRoom = 0f;
        lookTimer = 0f;
        currentCooldown = baseCooldown;
        currentState = State.Waiting;
        if (dollVisuals != null) dollVisuals.SetActive(true);
        canAttack = true;
    }

    private void Update()
    {
        if (target == null) return;

        // Siempre mira al jugador
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0f; 
        if (lookDirection.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        if (!canAttack) return;

        timeInRoom += Time.deltaTime;
        currentCooldown = Mathf.Max(minCooldown, baseCooldown - (timeInRoom * escalationRate));

        if (currentState == State.Waiting)
        {
            if (IsPlayerLookingAtMe())
            {
                // Tiempo de mirada
                lookTimer += Time.deltaTime; 
                
                if (lookTimer >= lookDurationRequired)
                {
                    lookTimer = 0f;
                    StartCoroutine(JumpscareAttack());
                }
            }
            else
            {
                lookTimer = 0f; // Si dejas de mirarla, el temporizador se limpia
            }
        }
    }

    private bool IsPlayerLookingAtMe()
    {
        Vector3 screenPoint = playerCam.WorldToViewportPoint(transform.position);
        bool inScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        if (!inScreen) return false;

        Ray ray = new Ray(playerCam.transform.position, transform.position - playerCam.transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator JumpscareAttack()
    {
        canAttack = false;
        
        if (dollVisuals != null) dollVisuals.SetActive(false);
        yield return new WaitForSeconds(0.2f);

        Vector3 spawnPosition = target.position + target.forward * 1.5f;
        agent.Warp(spawnPosition); 
        if (dollVisuals != null) dollVisuals.SetActive(true);

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        if (playerSanity == null) playerSanity = target.GetComponent<PlayerSanity>();
        playerSanity.TakeDamage(damageAmount);
        if (impulse != null) impulse.GenerateImpulse();

        yield return new WaitForSeconds(0.6f); 

        currentState = State.Retreating;
        if (dollVisuals != null) dollVisuals.SetActive(false); 

        Vector3 retreatDir = (transform.position - target.position).normalized;
        Vector3 retreatPoint = transform.position + retreatDir * retreatDistance;
        agent.Warp(retreatPoint); 

        yield return new WaitForSeconds(currentCooldown);

        if (dollVisuals != null) dollVisuals.SetActive(true);
        currentState = State.Waiting;
        canAttack = true;
    }
}
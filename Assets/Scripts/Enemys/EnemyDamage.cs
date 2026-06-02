using UnityEngine;

[DisallowMultipleComponent]
public class EnemyDamage : MonoBehaviour
{
    [Header("Ajustes de Daño")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageCooldown = 1.5f;

    private float nextDamageTime;
    private Animator animator;
    private PlayerSanity playerSanity; // Guardamos la referencia mientras esté dentro

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Si el jugador está dentro de la zona de ataque y ha pasado el cooldown...
        if (playerSanity != null && Time.time >= nextDamageTime)
        {
            EjecutarAtaque();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // El jugador ha entrado en el escudo, memorizamos su script de cordura
            playerSanity = other.GetComponent<PlayerSanity>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // El jugador se ha alejado, olvidamos la referencia para dejar de atacar
            playerSanity = null;
        }
    }

    private void EjecutarAtaque()
    {
        playerSanity.TakeDamage(damageAmount);
        nextDamageTime = Time.time + damageCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }
}
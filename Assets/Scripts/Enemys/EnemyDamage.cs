using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damageAmount = 10f;


    //enemigos fantasmas, ver si separar daños (alguno más fuerte)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSanity sanity = other.GetComponent<PlayerSanity>();
            if (sanity != null)
            {
                sanity.TakeDamage(damageAmount);
            }
        }
    }

    //enemigos no fantasmas u objetos de daño - ver!
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisionando con player");

            PlayerSanity sanity = collision.gameObject.GetComponent<PlayerSanity>();
            if (sanity != null)
            {
                sanity.TakeDamage(damageAmount);
            }
        }
    }
    
}
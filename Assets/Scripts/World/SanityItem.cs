using UnityEngine;

public class SanityItem : MonoBehaviour
{
    [Tooltip("Cantidad de cordura que recupera al recogerlo")]
    [SerializeField] private float sanityAmount = 25f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSanity playerSanity = other.GetComponent<PlayerSanity>();
            
            if (playerSanity != null)
            {
                playerSanity.Heal(sanityAmount);
                
                Debug.Log($"<color=blue>[[SanityItem]] Cordura recuperada: {sanityAmount}</color>");
                
                Destroy(gameObject);
            }
        }
    }
}
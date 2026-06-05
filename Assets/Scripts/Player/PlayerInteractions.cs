using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    public InputActionReference interactAction; 
    private PlayerSanity playerSanity; // Guardamos referencia para curarnos al hacer clic

    private void Awake()
    {
        playerSanity = GetComponent<PlayerSanity>();
    }

    private void OnEnable() => interactAction.action.Enable();
    private void OnDisable() => interactAction.action.Disable();
    
    private void OnTriggerEnter(Collider other)
    {
        // Las zonas invisibles y puertas siguen funcionando al caminar
        if (other.CompareTag("Door1") || other.CompareTag("ReturnToHab1"))
        {    
            WorldSceneManager.Instance.ProcessInteraction(other.tag);
        }       
    }

    private void Update()
    {
        if (interactAction != null && interactAction.action.WasPressedThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Mostrar mensaje en la UI de lo que tocamos
                if (MessageManager.Instance != null)
                {
                    MessageManager.Instance.Show(hit.collider.tag);
                }

                if (hit.collider.CompareTag("Doll"))
                {
                    WorldSceneManager.Instance.ProcessInteraction("Doll");
                    Destroy(hit.collider.gameObject); 
                }

                if (hit.collider.CompareTag("Key"))
                {
                    WorldSceneManager.Instance.ProcessInteraction("Key");
                    Destroy(hit.collider.gameObject); 
                }

                if (hit.collider.CompareTag("FinalNote"))
                {
                    WorldSceneManager.Instance.ProcessInteraction("FinalNote");
                }

                if (hit.collider.CompareTag("ZoneCheck"))
                {
                    WorldSceneManager.Instance.ProcessInteraction("ZoneCheck");
                    
                    Animator anim = hit.collider.GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.SetBool("Open", true); 
                    }
                    else
                    {
                        Debug.LogWarning("El objeto ZoneCheck no tiene un componente Animator.");
                    }

                    hit.collider.enabled = false;
                }

                if (hit.collider.CompareTag("SanityItem"))
                {
                    if (playerSanity != null) 
                    {
                        playerSanity.Heal(25f);
                    }
                    Destroy(hit.collider.gameObject);
                }
            }
            else
            {

                if (MessageManager.Instance != null) 
                {
                    MessageManager.Instance.Show(null);
                }
            }
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    public InputActionReference interactAction; 
    private PlayerSanity playerSanity;

    private void Awake()
    {
        playerSanity = GetComponent<PlayerSanity>();
    }

    private void OnEnable() => interactAction.action.Enable();
    private void OnDisable() => interactAction.action.Disable();
    
    private void OnTriggerEnter(Collider other)
    {
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
                string objectTag = hit.collider.tag;

                if (MessageManager.Instance != null)
                {
                    MessageManager.Instance.Show(objectTag);
                }

                switch (objectTag)
                {
                    case "Doll":
                        WorldSceneManager.Instance.ProcessInteraction("Doll");
                        Destroy(hit.collider.gameObject);
                        break;

                    case "Key":
                        WorldSceneManager.Instance.ProcessInteraction("Key");
                        Destroy(hit.collider.gameObject);
                        break;

                    case "FinalNote":
                        WorldSceneManager.Instance.ProcessInteraction("FinalNote");
                        break;

                    case "ZoneCheck":
                        WorldSceneManager.Instance.ProcessInteraction("ZoneCheck");
                        Animator anim = hit.collider.GetComponent<Animator>();
                        if (anim != null) anim.SetBool("Open", true); 
                        hit.collider.enabled = false;
                        break;

                    case "SanityItem":
                        if (playerSanity != null) playerSanity.Heal(25f);
                        Destroy(hit.collider.gameObject);
                        break;
                }
            }
        }
    }
}
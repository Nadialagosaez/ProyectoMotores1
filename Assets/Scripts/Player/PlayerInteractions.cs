using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    public InputActionReference interactAction; 
    private PlayerSanity playerSanity;
    private int countUntag = 0;

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
               string objectTag = hit.collider.tag;

                switch (objectTag)
                {
                    case "Doll":
                        if (MessageManager.Instance != null) MessageManager.Instance.Show(objectTag);
                        WorldSceneManager.Instance.ProcessInteraction("Doll");
                        Destroy(hit.collider.gameObject);
                        break;

                    case "Key":
                        if (MessageManager.Instance != null) MessageManager.Instance.Show(objectTag);
                        WorldSceneManager.Instance.ProcessInteraction("Key");
                        Destroy(hit.collider.gameObject);
                        break;

                    case "FinalNote":
                        if (MessageManager.Instance != null) MessageManager.Instance.Show(objectTag);
                        WorldSceneManager.Instance.ProcessInteraction("FinalNote");
                        break;

                    case "ZoneCheck":
                        if (MessageManager.Instance != null) MessageManager.Instance.Show(objectTag);
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
                        break;

                    case "SanityItem":
                        if (MessageManager.Instance != null) MessageManager.Instance.Show(objectTag);
                        if (playerSanity != null) 
                        {
                            playerSanity.Heal(25f);
                        }
                        Destroy(hit.collider.gameObject);
                        break;

                    default:
                        if (MessageManager.Instance != null) 
                        {
                            if (countUntag < 10)
                            {
                                countUntag++;
                            }
                            else
                            {
                                MessageManager.Instance.Show("Untagged");
                                countUntag = 0;
                            }
                        }
                        break;
                }
            }
            else
            {
                 if (MessageManager.Instance != null) MessageManager.Instance.Show(null);
            }
        }
    }
}
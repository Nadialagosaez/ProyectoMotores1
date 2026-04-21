using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    public InputActionReference interactAction; 

    private void OnEnable() => interactAction.action.Enable();
    private void OnDisable() => interactAction.action.Disable();
    
    private void OnTriggerEnter(Collider other)
    {
        WorldSceneManager.Instance.ProcessInteraction(other.tag);

        if (other.CompareTag("Key"))
        {
            Destroy(other.gameObject);
        }
    }



    private void Update()
    {
        
    if (interactAction != null && interactAction.action.WasPressedThisFrame())
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //quitar para entrega final
            Debug.Log("He tocado un objeto llamado: " + hit.collider.name + " con el Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("FinalNote"))
            {
                WorldSceneManager.Instance.ProcessInteraction("FinalNote");
            }

            //aquí tambien iria la lógica para abrir las puertas con alguna animación
            
        }
        else 
        {
            //quitar para entrega final
            Debug.Log("El rayo no ha tocado nada.");
        }
    }
}
}
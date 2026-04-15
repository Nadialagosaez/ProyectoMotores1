using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] private WorldState worldState; 

    private void OnTriggerEnter(Collider other)
    {
        // Si toca un objeto interactuable (ej. una nota)
        // if (other.CompareTag("NoteWall"))
        // {
        //     worldState.noteWall = true;
        //     Debug.Log("Estado actualizado: Nota leída.");
        // }

        // Si toca un trigger de cambio de zona
        if (other.CompareTag("Door1"))
        {
            // En lugar de cargar aquí, llamamos al controlador central
            // que creamos en la respuesta anterior
            Debug.Log("Entrando a la siguiente habitación...");
            string scene = NextScene();
            WorldSceneManager.Instance.ChangeRoom(scene);
        }
    }

    private string NextScene()
    {
        return "Hab2";
        // Aquí es donde ocurre la magia del ScriptableObject
        // if (worldState.noteWall)
        // {
        //     return "Habitacion_Con_Secreto";
        // }
        // return "Habitacion_Normal";
    }
}
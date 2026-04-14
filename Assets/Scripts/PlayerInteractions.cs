using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class PlayerInteractions : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string nextSceneName;
    private bool sceneLoaded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door1")&& !sceneLoaded)
        {
            Debug.Log("Cruzó la puerta, cargando escena...");
            CargarEscenaAditiva();
        }
    }

    private void CargarEscenaAditiva()
    {
        // LoadSceneMode.Additive permite que la escena actual no se borre
        SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        sceneLoaded = true;
        Debug.Log("Cargando escena: " + nextSceneName);
    }
}
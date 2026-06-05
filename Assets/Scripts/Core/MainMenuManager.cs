using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Estado del Juego")]
    [SerializeField] private WorldState worldState;

    [Header("UI de Pausa")]
    [SerializeField] private GameObject pausePanel; 

    [SerializeField] private CinemachineCamera vCam;
    private bool isPaused = false;

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)        
            {
                Pause();
            }
        }
    }

    public void ClickPlay()
    {
        Debug.Log("Play button clicked");

        if (worldState != null)
        {
            worldState.ResetState();
            Debug.Log("WorldState inicializado correctamente.");
        }
        else
        {
            Debug.LogWarning("¡Atención! No has asignado el archivo WorldState en el Inspector.");
        }

        Time.timeScale = 1f; 

        StartCoroutine(LoadGameRoutine());
    }

    private IEnumerator LoadGameRoutine()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("MasterScene");
        while (!op.isDone) yield return null;
    }

    public void Pause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (vCam != null) vCam.enabled = false;
            
            if (pausePanel != null) pausePanel.SetActive(true);
            
            
            Debug.Log("Juego Pausado");
        }
        else
        {
            Time.timeScale = 1f; 
            if (vCam != null) vCam.enabled = true;

            if (pausePanel != null) pausePanel.SetActive(false); // Oculta el cartel
            
            
            Debug.Log("Juego Reanudado");
        }
    }

    public void ClickMenu()
    {
        Debug.Log("Menu button clicked");
        
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene("Menu");
    }
}
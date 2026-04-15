using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WorldSceneManager : MonoBehaviour
{
    public static WorldSceneManager Instance;
    public WorldState worldState; 

    void Awake()
    {
        if (Instance == null) 
    {
        Instance = this;
        // LIMPIA LOS DATOS AL DARLE AL PLAY
        if (worldState != null) worldState.ResetState(); 
    }
    else 
    {
        Destroy(gameObject);
    }
    }

   public void ChangeRoom(string newSceneName)
{
    if (newSceneName == worldState.currentRoomName) return;
    
    StartCoroutine(LoadSceneRoutine(newSceneName));
}

private IEnumerator LoadSceneRoutine(string sceneName)
{
    AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    
    while (!loadOp.isDone)
    {
        yield return null; 
    }

    // ⚠️ PASO EXTRA: Esperar un frame para que los objetos se inicialicen
    yield return new WaitForEndOfFrame();

    // Buscamos el spawn
    GameObject spawn = GameObject.Find("SpawnPoint");
    
    if (spawn != null) 
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // ⚠️ FIX PARA CHARACTER CONTROLLER:
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false; // Lo apagamos

            player.transform.position = spawn.transform.position;
            player.transform.rotation = spawn.transform.rotation;

            if (cc != null) cc.enabled = true; // Lo encendemos
            
            Debug.Log("Jugador teletransportado a: " + spawn.transform.position);
        }
    }
    else 
    {
        Debug.LogError("¡No encontré el SpawnPoint! Asegúrate de que se llame exactamente así.");
    }

    worldState.currentRoomName = sceneName;
}
}
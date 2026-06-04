using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WorldSceneManager : MonoBehaviour
{
    public static WorldSceneManager Instance;
    public WorldState worldState; 
    public ScreenFader fader;
    private bool isLoading = false; 

    void Awake()
    {
         if (SceneManager.sceneCount == 1) 
            {
                StartCoroutine(LoadSceneRoutine("Hab1"));
            }

        if (Instance == null) 
        { 
            Instance = this; 
            if (worldState != null) worldState.ResetState(); 
        }
        else Destroy(gameObject);
    }

public void ProcessInteraction(string tag)
    {
        string nextScene = "";

        switch (tag)
        {
            case "Door1": 
                Debug.Log("Saliendo Hab1");
                nextScene = CalculateNextSceneFromHab1();
                break;

            case "ReturnToHab1":
                Debug.Log("Checkeo si vuelvo a Hab1");
                nextScene = HandleReturnLogic();
                break;

            case "ZoneCheck":
                worldState.SetZoneCheck(true);
                Debug.Log("Zona investigada");
                return;

            case "Key":
                worldState.SetHasKey(true);
                Debug.Log("Tengo llave");
                return; 
            
            case "Doll":
                worldState.SetHasDoll(true);
                Debug.Log("Muñeca recogida, ahora puedes salir de Hab3");
                return;

            case "FinalNote":
                worldState.SetMsjRead(true);
                Debug.Log("Mensaje leido");
                return; 
        }

        if (!string.IsNullOrEmpty(nextScene)) 
            StartCoroutine(LoadSceneRoutine(nextScene));
    }

    private string CalculateNextSceneFromHab1()
    {
        if (worldState.hasKey)
        {
            return "Hab5";

        } else if (worldState.backFromHab3) return "Hab4";
        
        return "Hab2";
    }

     private string HandleReturnLogic()
    {
        string current = worldState.currentRoomName;

        if (current == "Hab2")
        {
            return worldState.zoneCheck ? "Hab3" : "Hab1";
        }

       if (current == "Hab3")
        {
            return worldState.hasDoll ? "Hab1" : "Hab4"; 
        }

        if (current == "Hab4")
        {
            return worldState.hasKey ? "Hab5" : "Hab1";
        }

        if (current == "Hab5")
        {
            if (worldState.msjRead)
            {
                return "WinScene";
            }
            //sin leer nota vuelvo a 1
            return "Hab1";
        }

        return "Hab1";
    }
    public IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName) || isLoading) yield break;
        isLoading = true;

        if (sceneName == "WinScene" || sceneName == "GameOverScene")
        {
            if (fader != null) 
            {
                yield return StartCoroutine(fader.FadeOut()); 
            }
            
            StopAllCoroutines();
            
            SceneManager.LoadScene(sceneName);
            yield break; 
        }

       
        string previousScene = worldState.currentRoomName;

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone) yield return null;
        
        yield return new WaitForEndOfFrame();

        HandlePlayerTeleport(sceneName); 

       
    
        if (!string.IsNullOrEmpty(previousScene) && previousScene != sceneName)
        {
            Scene sceneToUnload = SceneManager.GetSceneByName(previousScene);
            
            if (sceneToUnload.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(previousScene);
            }
            else
            {
                Debug.LogWarning("Se intentó descargar " + previousScene + " pero no estaba cargada.");
            }
        }
        worldState.SetCurrentRoomName(sceneName);

        if (sceneName == "Hab1")
        {
            ClockLoop clock = FindFirstObjectByType<ClockLoop>();
            if (clock != null)
            {
                clock.ReiniciarReloj();
            }
            if (worldState != null) // Asegúrate de tener la referencia a tu WorldState en este script
            {
                worldState.IncrementHab1Visits();
            }
        }
        
        if (fader != null) yield return StartCoroutine(fader.FadeIn());

        isLoading = false; 
    }

   
    private void HandlePlayerTeleport(string sceneName)
    {
        GameObject spawn = GameObject.Find("SpawnPoint" + sceneName);
        if (spawn == null) 
        {
            Debug.LogWarning("No se encontró el SpawnPoint para la escena: " + sceneName);
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; 
        }

        // Mantén esto por si acaso quedara algún CharacterController viejo en el proyecto
        // CharacterController cc = player.GetComponent<CharacterController>();
        // if (cc != null) cc.enabled = false; 

        player.transform.position = spawn.transform.position;
        player.transform.rotation = spawn.transform.rotation;

        // 3. Devolvemos el control al motor de físicas limpiando inercias anteriores
        //if (cc != null) cc.enabled = true;

        if (rb != null)
        {
            rb.isKinematic = false;
            
            rb.linearVelocity = Vector3.zero;  
            rb.angularVelocity = Vector3.zero;
            
            rb.position = spawn.transform.position;
            rb.rotation = spawn.transform.rotation;
        }
    }
}

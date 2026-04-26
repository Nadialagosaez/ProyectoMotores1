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
            //si pisó la zona de investigacion va a la 3 sino vuelve al inicio
            return worldState.zoneCheck ? "Hab3" : "Hab1";
        }

        if (current == "Hab3")
        {
            worldState.SetBackFromHab3(true);
            return "Hab1";
        }

        if (current == "Hab4")
        {
            //con llave se activa la 5 sino vuelve a la 1
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

        
        if (fader != null) yield return StartCoroutine(fader.FadeIn());

        isLoading = false; 
    }

   
    private void HandlePlayerTeleport(string sceneName)
    {
        GameObject spawn = GameObject.Find("SpawnPoint" + sceneName);
        if (spawn == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false; 

        player.transform.position = spawn.transform.position;
        player.transform.rotation = spawn.transform.rotation;

        if (cc != null) cc.enabled = true;
    }
}

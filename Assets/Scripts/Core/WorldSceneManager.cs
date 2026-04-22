using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;

public class WorldSceneManager : MonoBehaviour
{
    public static WorldSceneManager Instance;
    public WorldState worldState; 
    public ScreenFader fader;

    void Awake()
    {
         if (SceneManager.sceneCount == 1) 
            {
                StartCoroutine(LoadSceneWithDelay("Hab1"));
            }

        if (Instance == null) 
        { 
            Instance = this; 
            if (worldState != null) worldState.ResetState(); 
        }
        else Destroy(gameObject);
    }


    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForEndOfFrame();
        StartCoroutine(LoadSceneRoutine(sceneName));
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
        if (string.IsNullOrEmpty(sceneName)) yield break;
        
        if (sceneName == "WinScene")
        {
            Debug.Log("Inicia fundido");
            
            if (fader != null) 
                yield return StartCoroutine(fader.FadeOut()); 
            SceneManager.LoadScene("WinScene"); 
            yield break; 
        }
        
        if (sceneName == "GameOverScene")
        {
            Debug.Log("Inicia fundido");
            
            if (fader != null) 
                yield return StartCoroutine(fader.FadeOut()); 
            SceneManager.LoadScene("GameOverScene"); 
            yield break; 
        }
      
        string previousScene = worldState.currentRoomName;


        //carga aditiva de escena
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone) yield return null;
        
        yield return new WaitForEndOfFrame();

        //teletransporter
        GameObject spawn = GameObject.Find("SpawnPoint" + sceneName);
        if (spawn != null) 
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false; // lo deshabilito para el transporter

                player.transform.position = spawn.transform.position;
                player.transform.rotation = spawn.transform.rotation;

                if (cc != null) cc.enabled = true;
            }
        }

        //quito escena anterior
      if (!string.IsNullOrEmpty(previousScene) && previousScene != sceneName)
        {
            SceneManager.UnloadSceneAsync(previousScene);
        }
        worldState.SetCurrentRoomName(sceneName);

        if (fader != null) yield return StartCoroutine(fader.FadeIn());
        Debug.Log("Habitación lista, quitando fondo negro");
    }
}
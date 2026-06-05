using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WorldSceneManager : MonoBehaviour
{
    public static WorldSceneManager Instance;

    [Header("Configuración Global")]
    public WorldState worldState; 
    public ScreenFader fader;
    
    private bool isLoading = false; 
    public static bool IsGameReady = false;

    void Awake()
    {
        IsGameReady = false;
        
        if (Instance == null) 
        { 
            Instance = this;
            transform.SetParent(null); 
            DontDestroyOnLoad(gameObject);
            
            if (worldState != null) worldState.ResetState(); 
        }
        else 
        {
            Destroy(gameObject);
            return; 
        }

        if (SceneManager.sceneCount == 1) 
        {
            StartCoroutine(LoadSceneRoutine("Hab1"));
        }
    }

    public void ProcessInteraction(string tag)
    {
        string nextScene = "";

        switch (tag)
        {
            case "Door1": 
                nextScene = CalculateNextSceneFromHab1();
                break;
            case "ReturnToHab1":
                nextScene = HandleReturnLogic();
                break;
            case "ZoneCheck":
                worldState.SetZoneCheck(true);
                return;
            case "Key":
                worldState.SetHasKey(true);
                return; 
            case "Doll":
                worldState.SetHasDoll(true);
                return;
            case "FinalNote":
                worldState.SetMsjRead(true);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayFinalLoop();
                return; 
        }

        if (!string.IsNullOrEmpty(nextScene)) 
            StartCoroutine(LoadSceneRoutine(nextScene));
    }

    private string CalculateNextSceneFromHab1()
    {
        if (worldState.hasKey) return "Hab5";
        if (worldState.backFromHab3) return "Hab4";
        return "Hab2";
    }

    private string HandleReturnLogic()
    {
        string current = worldState.currentRoomName;
        if (current == "Hab2") return worldState.zoneCheck ? "Hab3" : "Hab1";
        if (current == "Hab3") { worldState.SetBackFromHab3(true); return "Hab1"; }
        if (current == "Hab4") return worldState.hasKey ? "Hab5" : "Hab1";
        if (current == "Hab5") return worldState.msjRead ? "WinScene" : "Hab1";
        return "Hab1";
    }

    public IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName) || isLoading) yield break;
        isLoading = true;

        if (sceneName == "WinScene" || sceneName == "GameOverScene")
        {
            if (fader != null) yield return StartCoroutine(fader.FadeOut());
            if (AudioManager.Instance != null) AudioManager.Instance.StopMusic();
            
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
            if (sceneToUnload.isLoaded) yield return SceneManager.UnloadSceneAsync(previousScene);
        }

        worldState.SetCurrentRoomName(sceneName);

        if (sceneName == "Hab1")
        {
            ClockLoop clock = FindFirstObjectByType<ClockLoop>();
            if (clock != null) clock.ReiniciarReloj();
            worldState.IncrementHab1Visits();
        }
        
        if (fader != null) yield return StartCoroutine(fader.FadeIn());
        
        IsGameReady = true;

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
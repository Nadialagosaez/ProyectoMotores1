using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
//    public GameObject blackOverlay; 

    public void ClickPlay()
    {
        Debug.Log("Play button clicked");
        StartCoroutine(LoadGameRoutine());
    }

    private IEnumerator LoadGameRoutine()
    {

        AsyncOperation op = SceneManager.LoadSceneAsync("MasterScene");
        
        while (!op.isDone) yield return null;

    }

    public void ClickMenu()
    {
        Debug.Log("Menu button clicked");
        SceneManager.LoadScene("Menu");
    }
}
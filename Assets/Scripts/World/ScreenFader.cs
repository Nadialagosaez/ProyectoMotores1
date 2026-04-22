using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeDuration = 1.0f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // Empezamos en negro total para cubrir la carga inicial
        canvasGroup.alpha = 1; 
    }

    public IEnumerator FadeIn() // De negro a transparente
    {
        yield return StartCoroutine(Fade(1, 0));
    }

    public IEnumerator FadeOut() // De transparente a negro
    {
        yield return StartCoroutine(Fade(0, 1));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }
}
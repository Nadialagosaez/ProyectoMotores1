using UnityEngine;
using TMPro;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;
    public float displayDuration = 2f;

    public void Play(string message)
    {
        if (messageText == null) messageText = GetComponent<TextMeshProUGUI>();
        
        if (messageText != null) 
        {
            messageText.text = message;
            Debug.Log("Mensaje mostrado: " + message);
        }
        else 
        {
            Debug.LogError("MessageUI: No se encontró TextMeshProUGUI");
        }
        
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        Destroy(gameObject, displayDuration);
    }
}

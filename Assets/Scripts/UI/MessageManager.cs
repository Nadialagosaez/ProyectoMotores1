using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct TagMessage
{
    public string tag;
    [TextArea]
    public string message;
}

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private Transform uiParent;

    [Header("Messages")]
    [SerializeField] private List<TagMessage> tagMessagesList = new List<TagMessage>();
    [SerializeField] private string defaultMessage = "Debo encontrar la salida...";
    [SerializeField] private List<string> noTagMessages = new List<string> { "No hay nada aquí", "Sigue buscando...", "Todavía nada", "Creo que me vigilan...", "¿Dónde estoy?" };

    [Header("Mensajes de Daño/Cordura")]
    [SerializeField] private List<string> damageMessages = new List<string> {"¡Déjame en paz!", "¿Qué ha sido eso?", "¡No puedo soportarlo!", "Perderé la cabeza...", "Quien soy?"};

    private Dictionary<string, string> tagMessages = new Dictionary<string, string>();
    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowing = false;
    private float messageEndTime = 0f;

    // Controles de repetición
    private bool hasShownDefaultNoTag = false;
    private string lastNoTagMessage = null;
    private string lastDamageMessage = null; 

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        { 
            Debug.LogWarning("Ya existe una instancia de MessageManager. Destruyendo el duplicado.");
            Destroy(gameObject); 
            return; 
        }

        foreach (var tm in tagMessagesList)
        {
            if (!string.IsNullOrEmpty(tm.tag)) 
            {
                tagMessages[tm.tag] = tm.message;
            }
        }
    }

    void Update()
    {
        if (isShowing && Time.time >= messageEndTime)
        {
            isShowing = false;
            DisplayNextMessage();
        }
    }

    public void Show(string tag)
    {
        string messageToShow;

        if (!string.IsNullOrEmpty(tag) && tagMessages.TryGetValue(tag, out string taggedMsg))
        {
            messageToShow = taggedMsg;
        }
        else 
        {
            if (!hasShownDefaultNoTag)
            {
                messageToShow = defaultMessage;
                hasShownDefaultNoTag = true;
                lastNoTagMessage = defaultMessage; 

                if (!noTagMessages.Contains(defaultMessage)) noTagMessages.Add(defaultMessage);
            }
            else
            {
                messageToShow = GetRandomNoTagMessage();
            }
        }

        EnqueueAndTryShow(messageToShow);
    }

    public void ShowDamageMessage()
    {
        if (damageMessages == null || damageMessages.Count == 0) return;

        string messageToShow;

        if (damageMessages.Count == 1)
        {
            messageToShow = damageMessages[0];
        }
        else
        {
            int index = Random.Range(0, damageMessages.Count);
            
            while (damageMessages[index] == lastDamageMessage)
            {
                index = Random.Range(0, damageMessages.Count);
            }

            lastDamageMessage = damageMessages[index];
            messageToShow = lastDamageMessage;
        }

        EnqueueAndTryShow(messageToShow);
    }

    private string GetRandomNoTagMessage()
    {
        if (noTagMessages == null || noTagMessages.Count == 0) return defaultMessage;
        if (noTagMessages.Count == 1) return noTagMessages[0];

        int index = Random.Range(0, noTagMessages.Count);
        while (noTagMessages[index] == lastNoTagMessage)
        {
            index = Random.Range(0, noTagMessages.Count);
        }

        lastNoTagMessage = noTagMessages[index];
        return lastNoTagMessage;
    }

    private void EnqueueAndTryShow(string msg)
    {
        messageQueue.Enqueue(msg);
        if (!isShowing)
        {
            DisplayNextMessage();
        }
    }

    private void DisplayNextMessage()
    {
        if (messageQueue.Count == 0) return;

        string msg = messageQueue.Dequeue();
        
        if (messagePrefab == null) return;

        if (uiParent == null)
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null) uiParent = canvas.transform;
        }

        GameObject go = Instantiate(messagePrefab, uiParent);
        var ui = go.GetComponent<MessageUI>();
        
        if (ui != null)
        {
            isShowing = true;
            messageEndTime = Time.time + ui.displayDuration;
            ui.Play(msg);
        }
        else
        {
            Destroy(go);
        }
    }
}
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
    [SerializeField] private List<string> noTagMessages = new List<string> { "No hay nada aquí.", "Sigue buscando...", "Todavía nada." };

    private Dictionary<string, string> tagMessages = new Dictionary<string, string>();
    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowing = false;
    private float messageEndTime = 0f;

    // Control de llamadas sin tag
    private bool hasShownDefaultNoTag = false;
    private string lastNoTagMessage = null;

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

        // diccionario de tags
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
        // Verificar si el mensaje actual terminó para lanzar el siguiente
        if (isShowing && Time.time >= messageEndTime)
        {
            isShowing = false;
            DisplayNextMessage();
        }
    }

public void Show(string tag)
    {
        string messageToShow;

        // 1. Si llega un tag válido y está en la lista, lo mostramos
        if (!string.IsNullOrEmpty(tag) && tagMessages.TryGetValue(tag, out string taggedMsg))
        {
            messageToShow = taggedMsg;
        }
        // 2. Si llega nulo, vacío o un tag que no existe
        else 
        {
            if (!hasShownDefaultNoTag)
            {
                messageToShow = defaultMessage;
                hasShownDefaultNoTag = true;
                
                // Guardo el msj x defecto en la lista 
                lastNoTagMessage = defaultMessage; 

                if (!noTagMessages.Contains(defaultMessage))
                {
                    noTagMessages.Add(defaultMessage);
                }
            }
            else
            {
                messageToShow = GetRandomNoTagMessage();
            }
        }

        messageQueue.Enqueue(messageToShow);
        
        // Si no hay nada mostrándose actualmente, forzar a mostrar
        if (!isShowing)
        {
            DisplayNextMessage();
        }
    }

    private string GetRandomNoTagMessage()
    {
        if (noTagMessages == null || noTagMessages.Count == 0) return defaultMessage;
        
        if (noTagMessages.Count == 1) return noTagMessages[0];

        int index = Random.Range(0, noTagMessages.Count);
        
        // No repito último mensaje mostrado
        while (noTagMessages[index] == lastNoTagMessage)
        {
            index = Random.Range(0, noTagMessages.Count);
        }

        lastNoTagMessage = noTagMessages[index];
        return lastNoTagMessage;
    }

    private void DisplayNextMessage()
    {
        // Si no hay mensajes en la cola, salimos
        if (messageQueue.Count == 0) return;

        string msg = messageQueue.Dequeue();
        
        if (messagePrefab == null)
        {
            Debug.LogWarning("MessageManager: messagePrefab no está asignado en el Inspector.");
            return;
        }

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
            Debug.LogError("MessageUI no encontrado en el prefab instanciado.");
            Destroy(go);
        }
    }
}
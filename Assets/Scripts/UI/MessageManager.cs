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
    [SerializeField] private WorldState worldState;

    [Header("Messages")]
    [SerializeField] private List<TagMessage> tagMessagesList = new List<TagMessage>();
    [SerializeField] private string defaultMessage = "Debo encontrar la salida...";
    [SerializeField] private List<string> noTagMessages = new List<string> { "No hay nada aquí...", "Sigue buscando...", "Todavía nada..." };

    [Header("Messages on Damage")]
    [SerializeField] private List<string> damageMessages = new List<string> {"Déjame en paz!", "Qué ha sido eso?", "Debo salir de aquí!"};

    [Header("Generic Clicks")]
    [SerializeField] private int necesaryClicks = 10;
    private int counterClicks = 0;

   [SerializeField] private PlayerSanity playerSanity;

    private Dictionary<string, string> tagMessages = new Dictionary<string, string>();
    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowing = false;
    private float messageEndTime = 0f;
    private string currentActiveMessage = ""; 

    private bool hasShownDefaultNoTag = false;
    private string lastNoTagMessage = "";
    private string lastDamageMessage = ""; 

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        { 
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
            currentActiveMessage = "";
            DisplayNextMessage();
        }
    }

    public void Show(string tag)
    {
        if (!string.IsNullOrEmpty(tag) && tagMessages.TryGetValue(tag, out string taggedMsg))
        {
            if (tag == "Clock" && worldState != null && worldState.hab1Visits > 1)
            {
                taggedMsg = "Parece que el tiempo no pasa aquí...";
            }

            if (tag == "SanityItem" && playerSanity != null && playerSanity.CurrentSanity < 100)
            {
                taggedMsg = "Me siento mejor con esto...";
            }

            EnqueueAndTryShow(taggedMsg);
            return; 
        }

        counterClicks++;

        if (counterClicks < necesaryClicks)
        {
            return;
        }

        counterClicks = 0;
        
        string messageToShow = GetGenericMessage();
        EnqueueAndTryShow(messageToShow);
    }

    private string GetGenericMessage()
    {
        if (!hasShownDefaultNoTag)
        {
            hasShownDefaultNoTag = true;
            return defaultMessage;
        }

        if (noTagMessages.Count == 0) return defaultMessage;

        int index = Random.Range(0, noTagMessages.Count);
        string randomMsg = noTagMessages[index];

        if (randomMsg == lastNoTagMessage && noTagMessages.Count > 1)
        {
            index = (index + 1) % noTagMessages.Count;
            randomMsg = noTagMessages[index];
        }

        lastNoTagMessage = randomMsg;
        return randomMsg;
    }

    public void ShowDamageMessage()
    {
        if (damageMessages == null || damageMessages.Count == 0) return;

        int index = Random.Range(0, damageMessages.Count);
        string dmgMsg = damageMessages[index];

        if (dmgMsg == lastDamageMessage && damageMessages.Count > 1)
        {
            index = (index + 1) % damageMessages.Count;
            dmgMsg = damageMessages[index];
        }

        lastDamageMessage = dmgMsg;
        EnqueueAndTryShow(dmgMsg);
    }

    private void EnqueueAndTryShow(string msg)
    {
        if (currentActiveMessage == msg || messageQueue.Contains(msg)) return;

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
            currentActiveMessage = msg;
            messageEndTime = Time.time + ui.displayDuration;
            ui.Play(msg);
        }
        else
        {
            Destroy(go);
        }
    }

    public void UpdateTagMessage(string tag, string newMessage)
    {
        if (tagMessages.ContainsKey(tag)) tagMessages[tag] = newMessage;
        else tagMessages.Add(tag, newMessage);

        for (int i = 0; i < tagMessagesList.Count; i++)
        {
            if (tagMessagesList[i].tag == tag)
            {
                TagMessage updatedStruct = tagMessagesList[i];
                updatedStruct.message = newMessage;
                tagMessagesList[i] = updatedStruct;
                break;
            }
        }
    }
}
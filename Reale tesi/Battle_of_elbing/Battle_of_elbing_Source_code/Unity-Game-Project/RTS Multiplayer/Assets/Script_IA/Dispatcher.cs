using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] 
public class Message 
{
    public string messageType;
    public int objectiveId;
}

public class Dispatcher : MonoBehaviour
{
    // SINGLETON PATTERN: Rende il Dispatcher accessibile globalmente e unico
    public static Dispatcher Instance { get; private set; }

    [Header("Bacheca Messaggi")]
    public List<Message> activeMessages = new List<Message>();

    [Header("Configurazione")]
    public float messageDuration = 5.0f; 

    void Awake()
    {
        // Gestione del Singleton per evitare duplicati nei cambi scena
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Impedisce a Unity di distruggerlo quando cambi scena!
        }
        else
        {
            Destroy(gameObject); // Se ne esiste già uno, distruggi il doppione
        }
    }

    void Start() 
    {
        Debug.Log("Inizio stress test Dispatcher: invio 10 messaggi...");
        for (int i = 0; i < 10; i++)
        {
            string tipoTest = (i % 2 == 0) ? "UnderAttack" : "NeedBackup";
            PostMessage(tipoTest, i);
        }
    }

    public void PostMessage(string type, int objId)
    {
        Message msg = new Message { messageType = type, objectiveId = objId };
        activeMessages.Add(msg);
        
        Debug.Log($"[DISPATCHER] Nuovo messaggio: {type} all'obiettivo {objId}");
        StartCoroutine(RemoveMessageAfterDelay(msg, messageDuration));
    }

    private IEnumerator RemoveMessageAfterDelay(Message msg, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (activeMessages.Contains(msg))
        {
            activeMessages.Remove(msg);
            Debug.Log($"[DISPATCHER] Messaggio scaduto e rimosso: {msg.messageType}");
        }
    }

    public void ClearMessages()
    {
        activeMessages.Clear();
    }

    void OnDisable() // TODO rimuovere dopo test
    {
        Debug.LogError($"[DEBUG] Il Dispatcher è stato DISATTIVATO!", this);
    }

    void OnDestroy() // TODO rimuovere dopo test
    {
        Debug.LogError($"[DEBUG] Il Dispatcher è stato DISTRUTTO!", this);
    }
}
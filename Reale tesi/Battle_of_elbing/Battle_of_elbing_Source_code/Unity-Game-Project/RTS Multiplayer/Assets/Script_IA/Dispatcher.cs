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

    // Start rimosso o tenuto pulito per l'inizializzazione reale
    void Start() 
    {
        Debug.Log("[DISPATCHER] Inizializzato con successo e pronto a ricevere messaggi.");
    }

    public void PostMessage(string type, int objId)
    {
        Message msg = new Message { messageType = type, objectiveId = objId };
        activeMessages.Add(msg);
        
        Debug.Log($"[DISPATCHER] Nuovo messaggio ricevuto: {type} per l'obiettivo {objId}");
        StartCoroutine(RemoveMessageAfterDelay(msg, messageDuration));
    }

    private IEnumerator RemoveMessageAfterDelay(Message msg, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (activeMessages.Contains(msg))
        {
            activeMessages.Remove(msg);
            Debug.Log($"[DISPATCHER] Messaggio scaduto e rimosso: {msg.messageType} (Obiettivo: {msg.objectiveId})");
        }
    }

    public void ClearMessages()
    {
        activeMessages.Clear();
        Debug.Log("[DISPATCHER] Bacheca messaggi svuotata manualmente.");
    }
}
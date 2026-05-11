using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] 
public class Message //penso sia piu giusto inserire il ttl qui in base al tipo di messaggio
{
    public string messageType;
    public int objectiveId;
}

public class Dispatcher : MonoBehaviour
{
    [Header("Bacheca Messaggi")]
    public List<Message> activeMessages = new List<Message>();

    [Header("Configurazione")]
    public float messageDuration = 5.0f; // I messaggi spariscono dopo 5 secondi da capire un buon tempo per farli sparire 

    void Start() {
        Debug.Log("Inizio stress test Dispatcher: invio 10 messaggi...");

        for (int i = 0; i < 10; i++){
        // Inviamo messaggi su obiettivi diversi (0-9) con tipi diversi
        string tipoTest = (i % 2 == 0) ? "UnderAttack" : "NeedBackup";
        PostMessage(tipoTest, i);
        }
    }
    public void PostMessage(string type, int objId)
    {
        Message msg = new Message { messageType = type, objectiveId = objId };
        activeMessages.Add(msg);
        
        Debug.Log($"[DISPATCHER] Nuovo messaggio: {type} all'obiettivo {objId}");

        // AVVIAMO IL TIMER: questo messaggio si auto-distruggerà
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
}
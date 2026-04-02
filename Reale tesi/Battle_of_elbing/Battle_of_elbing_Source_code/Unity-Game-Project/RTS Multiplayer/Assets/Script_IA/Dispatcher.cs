using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Definiamo come è fatto un singolo messaggio
[System.Serializable]
public class Message
{
    public string messageType; // Es: "UnderAttack", "NeedBackup"
    public int objectiveId;    // Es: 1, 2, 3...
}

public class Dispatcher : MonoBehaviour
{
    [Header("Bacheca Messaggi per ASP (Sola Lettura)")]
    // ThinkEngine è in grado di leggere le liste di C#!
    public List<Message> activeMessages = new List<Message>();

    // Il metodo che useranno i soldati per "appendere un post-it"
    public void PostMessage(string type, int objId)
    {
        Message msg = new Message();
        msg.messageType = type;
        msg.objectiveId = objId;
        
        activeMessages.Add(msg);
        Debug.Log($"[DISPATCHER] Nuovo messaggio ricevuto: {type} all'obiettivo {objId}");
    }

    // Metodo opzionale per pulire la bacheca (es. ogni X secondi o a fine turno)
    public void ClearMessages()
    {
        activeMessages.Clear();
    }
}
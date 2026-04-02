using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectiveSensor : MonoBehaviour
{
    [Header("Identificativo per ASP")]
    // Questo è il nome che userà ASP per riconoscere di QUALE base stiamo parlando
    public string objectiveID = "objective_name"; 

    [Header("Dati per ASP")]

    // Qui scriveremo "NEUTRAL", "FRIENDLY" o "HOSTILE"

    public string currentOwner; 

    private ObjectiveInfluenceScript influenceScript;

    void Start()
    {
        // Peschiamo lo script originale che sta sullo stesso oggetto
        influenceScript = GetComponent<ObjectiveInfluenceScript>();
    }

    void Update()
    {
        // Copiamo lo stato originale in formato stringa (testo) che ASP digerisce perfettamente
        if (influenceScript != null)
        {
            currentOwner = influenceScript.influenceState.ToString();
        }
    }
}
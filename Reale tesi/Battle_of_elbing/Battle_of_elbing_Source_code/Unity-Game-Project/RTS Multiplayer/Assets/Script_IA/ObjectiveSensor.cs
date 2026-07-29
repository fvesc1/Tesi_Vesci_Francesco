using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSensor : MonoBehaviour
{
    [Header("Identificativo per ASP")]
    public string objectiveID = "objective_1"; 
    public int objectiveNumericID = 1; // ID Numerico corrispondente (1, 2, 3...)

    [Header("Dati per ASP")]
    public string currentOwner; 

    [Header("Parametri Guarnigione")]
    public float captureRadius = 10f; // Raggio di ricerca dei soldati nell'area

    private ObjectiveInfluenceScript influenceScript;
    private string lastOwner = "";

    void Start()
    {
        // Peschiamo lo script originale sullo stesso GameObject
        influenceScript = GetComponent<ObjectiveInfluenceScript>();
        if (influenceScript != null)
        {
            lastOwner = influenceScript.influenceState.ToString();
            currentOwner = lastOwner;
        }
    }

    void Update()
    {
        if (influenceScript != null)
        {
            currentOwner = influenceScript.influenceState.ToString();

            // Rileva il momento ESATTO della conquista (transizione a FRIENDLY)
            if (currentOwner == "FRIENDLY" && lastOwner != "FRIENDLY")
            {
                OnObjectiveCaptured();
            }

            lastOwner = currentOwner;
        }
    }

    private void OnObjectiveCaptured()
    {
        Debug.Log($"<color=green>[OBIETTIVO {objectiveID}]</color> Conquistato! Selezione guarnigione di difesa...");

        // 1. Cerchiamo tutti i soldati nell'area dell'obiettivo
        Collider[] colliders = Physics.OverlapSphere(transform.position, captureRadius);
        List<SoldierBrain> nearbySoldiers = new List<SoldierBrain>();

        foreach (var col in colliders)
        {
            SoldierBrain soldier = col.GetComponent<SoldierBrain>();
            // Prendiamo solo i nostri soldati che non stanno già difendendo
            if (soldier != null && !soldier.isDefending)
            {
                nearbySoldiers.Add(soldier);
            }
        }

        if (nearbySoldiers.Count == 0)
        {
            Debug.LogWarning($"[OBIETTIVO {objectiveID}] Nessun soldato disponibile nella zona per la difesa.");
            return;
        }

        // 2. Cerchiamo se tra i presenti c'è un Heavy
        SoldierBrain heavySoldier = nearbySoldiers.Find(s => IsHeavyUnit(s));

        if (heavySoldier != null)
        {
            // CASO A: Trovato un Heavy -> Ne basta 1 solo
            heavySoldier.AssignDefenseDuty(objectiveNumericID);
            Debug.Log($"<color=yellow>[OBIETTIVO {objectiveID}]</color> Assegnato 1 HEAVY a presidio.");
        }
        else
        {
            // CASO B: Nessun Heavy -> Prende fino a 2 soldati leggeri (Conscript / Sniper)
            int assignedCount = 0;
            foreach (var soldier in nearbySoldiers)
            {
                soldier.AssignDefenseDuty(objectiveNumericID);
                assignedCount++;
                if (assignedCount >= 2) break; // Massimo 2 soldati leggeri
            }
            Debug.Log($"<color=yellow>[OBIETTIVO {objectiveID}]</color> Assegnati {assignedCount} soldati leggeri a presidio.");
        }
    }

    private bool IsHeavyUnit(SoldierBrain soldier)
    {
        UnitScript unitScript = soldier.GetComponent<UnitScript>();
        if (unitScript != null && unitScript.unit != null)
        {
            // Verifica sul nome del SO o della classe dell'unità se e heavy
            return unitScript.unit.name.Contains("Heavy") || unitScript.unit.unitName.Contains("Heavy");
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, captureRadius);
    }
}
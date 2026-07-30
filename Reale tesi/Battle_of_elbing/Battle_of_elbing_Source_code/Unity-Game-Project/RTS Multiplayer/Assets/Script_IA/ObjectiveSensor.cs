using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSensor : MonoBehaviour
{
    [Header("Identificativo per ASP")]
    public string objectiveID = "objective_1"; 
    public int objectiveNumericID = 1;

    [Header("Dati per ASP")]
    public string currentOwner; 

    [Header("Parametri Guarnigione")]
    public float captureRadius = 10f;
    
    [Header("Gestione Linea del Fronte")]
    public List<ObjectiveSensor> neighboringObjectives = new List<ObjectiveSensor>();

    private ObjectiveInfluenceScript influenceScript;
    private string lastOwner = "";
    
    private List<SoldierBrain> activeDefenders = new List<SoldierBrain>(); //i soldati nell obiettivo per mettere is defending false
    private float frontlineCheckTimer = 0f;
    private float frontlineCheckInterval = 2f; 

    void Start()
    {
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

            // Rileva la cattura
            if (currentOwner == "FRIENDLY" && lastOwner != "FRIENDLY")
            {
                OnObjectiveCaptured();
            }
            lastOwner = currentOwner;
        }

        // Controllo Linea del Fronte (solo se siamo proprietari e abbiamo difensori e il timer e' scaduto
        if (currentOwner == "FRIENDLY" && activeDefenders.Count > 0)
        {
            frontlineCheckTimer += Time.deltaTime;
            if (frontlineCheckTimer >= frontlineCheckInterval)
            {
                frontlineCheckTimer = 0f;
                CheckFrontlineSafety();
            }
        }
    }

    private void OnObjectiveCaptured()
    {
        Debug.Log($"<color=green>[OBIETTIVO {objectiveID}]</color> Conquistato! Selezione guarnigione...");
        
        activeDefenders.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, captureRadius);
        List<SoldierBrain> nearbySoldiers = new List<SoldierBrain>();

        foreach (var col in colliders)
        {
            SoldierBrain soldier = col.GetComponent<SoldierBrain>();
            if (soldier != null && !soldier.isDefending)
            {
                nearbySoldiers.Add(soldier);
            }
        }

        if (nearbySoldiers.Count == 0) return;

        SoldierBrain heavySoldier = nearbySoldiers.Find(s => IsHeavyUnit(s));

        if (heavySoldier != null)
        {
            heavySoldier.AssignDefenseDuty(objectiveNumericID);
            activeDefenders.Add(heavySoldier);
            Debug.Log($"<color=yellow>[OBIETTIVO {objectiveID}]</color> Assegnato 1 HEAVY a presidio.");
        }
        else
        {
            int assignedCount = 0;
            foreach (var soldier in nearbySoldiers)
            {
                soldier.AssignDefenseDuty(objectiveNumericID);
                activeDefenders.Add(soldier);
                assignedCount++;
                if (assignedCount >= 2) break;
            }
            Debug.Log($"<color=yellow>[OBIETTIVO {objectiveID}]</color> Assegnati {assignedCount} soldati leggeri.");
        }
    }


    private void CheckFrontlineSafety() //se ho 2 vicini friendly allora dimetto i miei 2 (o 1 heavy) difensori
    {
        bool isSafe = true;

        if (neighboringObjectives.Count == 0) return;

        foreach (var neighbor in neighboringObjectives)
        {
            if (neighbor.currentOwner != "FRIENDLY")
            {
                isSafe = false;
                break;
            }
        }

        if (isSafe)
        {
            DismissDefenders();
        }
    }

    private void DismissDefenders()
    {
        Debug.Log($"<color=cyan>[OBIETTIVO {objectiveID}]</color> Zona sicura (vicini tutti FRIENDLY). Congedo guarnigione!");
        
        foreach (var defender in activeDefenders)
        {
            if (defender != null)
            {
                defender.ClearDefenseDuty();
            }
        }
        
        activeDefenders.Clear();
    }

    private bool IsHeavyUnit(SoldierBrain soldier)
    {
        UnitScript unitScript = soldier.GetComponent<UnitScript>();
        if (unitScript != null && unitScript.unit != null)
        {
            return unitScript.unit.name.Contains("Heavy") || unitScript.unit.unitName.Contains("Heavy");
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, captureRadius);

        Gizmos.color = Color.green;
        foreach(var neighbor in neighboringObjectives)
        {
            if(neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }
}
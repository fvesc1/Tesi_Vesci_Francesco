using System.Collections;
using System.Collections.Generic;


//per  ora non funziona a meno che non si aumenti il sight range a 1500

using UnityEngine;
using UnityEngine.AI;

public class SoldierBrain : MonoBehaviour
{
    [Header("Sensori e Raggio")]
    public float sightRange = 15f;
    public float attackRange = 5f;
    public string enemyTag = "Enemy";

    [Header("Sensori per ASP (Sola Lettura)")]
    public int myCurrentHealth;
    public float myHealthPercentage; //la percentuale e meglio del valore per asp soprattuto avendo conscript heavy e sniper
    public int visibleEnemiesCount;
    
    
    private NavMeshAgent agent;
    private Transform currentTarget;
    private AiTargetingSystem originalTargetingSystem;
    private UnitScript myUnitScript; //da qui prendo la vita della friendly unit

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalTargetingSystem = GetComponent<AiTargetingSystem>();
        
        // Colleghiamo il sensore della vita
        myUnitScript = GetComponent<UnitScript>();
    }

    void Update()
    {
        // 1. FASE SENSORI: Aggiorniamo i dati per ASP
        UpdateSensors();

        // 2. FASE ATTUATORI (Temporanea finché non c'è ASP)
        if (currentTarget == null)
        {
            SearchForEnemy();
        }
        else 
        {
            EngageEnemy();
        }
    }

    void UpdateSensors()
        {
            // 1. Sensore Vita
            if (myUnitScript != null && myUnitScript.unit != null)
            {
                myCurrentHealth = myUnitScript.currentHealth;
                myHealthPercentage = ((float)myCurrentHealth / myUnitScript.unit.health) * 100f;
            }

            // 2. Sensore Vista (Quanti nemici ci sono in zona?)
            visibleEnemiesCount = 0; // Azzeriamo il contatore ogni frame
            Collider[] hits = Physics.OverlapSphere(transform.position, sightRange);
            
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag(enemyTag))
                {
                    visibleEnemiesCount++; // Aggiungiamo 1 per ogni nemico trovato
                }
            }
        }
    void SearchForEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                currentTarget = hit.transform;
                if (originalTargetingSystem != null)
                {
                    originalTargetingSystem.target = currentTarget.gameObject;
                }
                break;
            }
        }
    }

    void EngageEnemy()
    {
        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);
        }
        else
        {
            agent.isStopped = true;
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
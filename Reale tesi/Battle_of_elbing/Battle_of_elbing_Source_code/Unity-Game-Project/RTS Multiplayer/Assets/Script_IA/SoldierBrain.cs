using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierBrain : MonoBehaviour
{
    [Header("Sensori e Raggio")]
    public float sightRange = 15f;
    public float attackRange = 5f;
    public string enemyTag = "Enemy";

    [Header("Sensori per ASP (Lettura)")]
    public int myCurrentHealth;
    public float myHealthPercentage; 
    public int visibleEnemiesCount;
    
    // --- NUOVE VARIABILI PER GLI ATTUATORI ---
    [Header("Attuatori da ASP (Scrittura)")]
    public bool hasAspOrder = false; // ASP lo imposta a true quando dà un ordine
    public float aspTargetX;
    public float aspTargetY;
    public float aspTargetZ;
    // ----------------------------------------
    
    private NavMeshAgent agent;
    private Transform currentTarget;
    private AiTargetingSystem originalTargetingSystem;
    private UnitScript myUnitScript;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalTargetingSystem = GetComponent<AiTargetingSystem>();
        myUnitScript = GetComponent<UnitScript>();
    }

    void Update()
    {
        // 1. FASE SENSORI: Aggiorniamo i dati per ASP
        UpdateSensors();

        // 2. FASE ATTUATORI: Chi comanda il movimento?
        if (hasAspOrder)
        {
            // Se ASP ha dettato delle coordinate, marcia verso quel punto!
            // (Il FightScript continuerà comunque a sparare in automatico se vede nemici per strada)
            agent.isStopped = false;
            agent.SetDestination(new Vector3(aspTargetX, aspTargetY, aspTargetZ));
        }
        else 
        {
            // Comportamento autonomo di base (se ASP non ha dato ordini)
            if (currentTarget == null)
            {
                SearchForEnemy();
            }
            else 
            {
                EngageEnemy();
            }
        }
    }

    void UpdateSensors()
    {
        if (myUnitScript != null && myUnitScript.unit != null)
        {
            myCurrentHealth = myUnitScript.currentHealth;
            myHealthPercentage = ((float)myCurrentHealth / myUnitScript.unit.health) * 100f;
        }

        visibleEnemiesCount = 0; 
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                visibleEnemiesCount++; 
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
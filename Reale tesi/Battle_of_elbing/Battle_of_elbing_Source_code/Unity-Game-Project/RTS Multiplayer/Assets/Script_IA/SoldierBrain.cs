using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierBrain : MonoBehaviour
{
    public Dispatcher globalDispatcher;

    [Header("Sensori e Raggio")]
    public float sightRange = 15f;
    public float attackRange = 5f;
    public string enemyTag = "Enemy";

    [Header("Sensori per ASP (Lettura)")]
    public int myCurrentHealth;
    public float myHealthPercentage; 
    public int visibleEnemiesCount;
    public int myUnitId;

    public bool isBackupRequested = false;    
    public bool amILowHealth = false;        
    public bool isAttackOrderPresent = false; 
    public int currentGlobalObjectiveId = -1; 

    [Header("Stato Difesa (Sensori per ASP gestiti dall'Obiettivo)")]
    public bool isDefending = false; 
    public int defendedObjectiveId = -1; 

    [Header("Attuatori da ASP (Scrittura)")]
    public bool hasAspOrder = false;      
    public int aspTargetObjectiveId = -1; 
    
    private NavMeshAgent agent;
    private Transform currentTarget;
    private AiTargetingSystem originalTargetingSystem;
    private UnitScript myUnitScript;

    // OTTIMIZZAZIONE: Per evitare micro-freeze
    private float sensorTimer = 0f;
    private float sensorInterval = 0.15f; // Esegue la scansione ~6 volte al secondo
    private int lastAssignedObjectiveId = -2;

    void Start()
    {
        globalDispatcher = Dispatcher.Instance;
        agent = GetComponent<NavMeshAgent>();
        originalTargetingSystem = GetComponent<AiTargetingSystem>();
        myUnitScript = GetComponent<UnitScript>();
        
        if (myUnitId == 0)
        {
            myUnitId = gameObject.GetInstanceID();
        }
    }

    void Update()
    {
        // 1. OTTIMIZZAZIONE: I sensori scansiscono il mondo a intervalli regolari
        sensorTimer += Time.deltaTime;
        if (sensorTimer >= sensorInterval)
        {
            sensorTimer = 0f;
            UpdateSensors();
            SearchForEnemy();
        }

        // 2. Logica di esecuzione ad ogni frame
        if (currentTarget != null)
        {
            EngageEnemy(); // Se vede un nemico, ignora tutto e combatte/scappa
        }
        else if (hasAspOrder && aspTargetObjectiveId >= 0)
        {
            MoveToAspObjective(); // Se non ha nemici, segue l'ordine dell'ASP
        }
        else
        {
            if (agent != null && agent.isOnNavMesh && !agent.isStopped)
            {
                agent.isStopped = true;
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
        amILowHealth = myHealthPercentage < 30f;

        // --- Lettura Dispatcher ---
        isBackupRequested = false;
        isAttackOrderPresent = false;
        currentGlobalObjectiveId = -1;

        if (globalDispatcher != null && globalDispatcher.activeMessages != null)
        {
            foreach (Message msg in globalDispatcher.activeMessages)
            {
                if (msg.messageType == "NeedBackup")
                {
                    isBackupRequested = true;
                }
                else if (msg.messageType == "Attack")
                {
                    isAttackOrderPresent = true;
                    currentGlobalObjectiveId = msg.objectiveId;
                }
            }
        }
    }

    public void AssignDefenseDuty(int objectiveId)
    {
        isDefending = true;
        defendedObjectiveId = objectiveId;
        Debug.Log($"<color=yellow>[SoldierBrain]</color> Unità {myUnitId} precettata per difendere l'obiettivo {objectiveId}");
    }

    public void ClearDefenseDuty()
    {
        isDefending = false;
        defendedObjectiveId = -1;
    }


    void SearchForEnemy()
    {
        visibleEnemiesCount = 0;
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRange);
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                visibleEnemiesCount++;
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = hit.transform;
                }
            }
        }

        currentTarget = closest;
        if (originalTargetingSystem != null)
        {
            originalTargetingSystem.target = (currentTarget != null) ? currentTarget.gameObject : null;
        }
    }

    void EngageEnemy()
    {
        if (currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (amILowHealth)
        {
            Vector3 retreatDir = (transform.position - currentTarget.position).normalized;
            Vector3 retreatPos = transform.position + retreatDir * 5f;
            agent.isStopped = false;
            agent.SetDestination(retreatPos);
            lastAssignedObjectiveId = -2; // Reset per quando uscirà dal combattimento
        }
        else if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);
            lastAssignedObjectiveId = -2;
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
    
    void MoveToAspObjective()
    {
        if (globalDispatcher == null) return;

        Transform targetPoint = globalDispatcher.GetObjective(aspTargetObjectiveId);
        if (targetPoint != null && agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPoint.position);
            lastAssignedObjectiveId = aspTargetObjectiveId;
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
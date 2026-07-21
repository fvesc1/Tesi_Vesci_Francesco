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

// Tutti questi flag possono attivarsi contemporaneamente sara il brain del soldato a decidere quale ascoltare
    public bool isBackupRequested = false;    // C'è un SOS in bacheca?
    public bool amILowHealth = false;         // Sto per morire?
    public bool isAttackOrderPresent = false; // Il Commander ha ordinato l'attacco?
    public int currentGlobalObjectiveId = -1; // Verso quale ID dobbiamo attaccare?

    [Header("Attuatori da ASP (Scrittura)")]
    public bool hasAspOrder = false;      // ASP ha preso il controllo? Nonostante cio se incontrassi un nemico sul mio percorso, lo ingaggeiro o farò retrofront
    public int aspTargetObjectiveId = -1; // Verso quale ID obiettivo devo andare?
    
    private NavMeshAgent agent;
    private Transform currentTarget;
    private AiTargetingSystem originalTargetingSystem;
    private UnitScript myUnitScript;

    void Start()
    {
        globalDispatcher = Dispatcher.Instance;
        agent = GetComponent<NavMeshAgent>();
        originalTargetingSystem = GetComponent<AiTargetingSystem>();
        myUnitScript = GetComponent<UnitScript>();
        myUnitId = gameObject.GetInstanceID();
    }

    void Update()
    {
        // 1. FASE SENSORI: Aggiorniamo i dati per darli in pasto all'ASP
        UpdateSensors();

        // 2. FASE ATTUATORI: Chi comanda il movimento?
        if (hasAspOrder && aspTargetObjectiveId != -1)
        {
            // L'ASP HA DECISO DI MUOVERCI (per attaccare o aiutare)
            agent.isStopped = false;
            
            // Chiediamo al Dispatcher le coordinate esatte di quell'Obiettivo (0-9)
            Transform targetPoint = globalDispatcher.GetObjective(aspTargetObjectiveId);
            
            if (targetPoint != null)
            {
                agent.SetDestination(targetPoint.position);
            }
        }
        else 
        {
            // COMPORTAMENTO AUTONOMO (Se l'ASP non ha dato ordini di movimento)
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
        // --- Aggiorno la Vita ---
        if (myUnitScript != null && myUnitScript.unit != null)
        {
            myCurrentHealth = myUnitScript.currentHealth;
            myHealthPercentage = ((float)myCurrentHealth / myUnitScript.unit.health) * 100f;
        }

        amILowHealth = myHealthPercentage < 30f;

        // --- Logica Invio SOS ---
        // Se la vita scende sotto il 70%, chiedo aiuto
        if (myHealthPercentage < 70f && Time.frameCount % 100 == 0) 
        {
            // Passiamo l'ID dell'obiettivo attuale per dire agli alleati DOVE siamo
            int idToPass = currentGlobalObjectiveId != -1 ? currentGlobalObjectiveId : 0;
            globalDispatcher.PostMessage("NeedBackup", idToPass); 
            
            Debug.Log($"<color=cyan>[SOLDIER-{myUnitId}]</color> Vita ({myHealthPercentage}%). Inviato SOS per area {idToPass}.");
        }

        // --- Aggiorno Nemici Visibili ---
        visibleEnemiesCount = 0; 
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                visibleEnemiesCount++; 
            }
        }

        // --- Lettura della Bacheca (Dispatcher) ---
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

    // --- METODI DI COMBATTIMENTO BASE ---
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
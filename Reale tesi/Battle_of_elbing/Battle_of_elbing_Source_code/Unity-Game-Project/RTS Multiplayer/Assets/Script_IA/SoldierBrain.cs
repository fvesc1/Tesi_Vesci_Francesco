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

    private NavMeshAgent agent;
    private Transform currentTarget;
    
    // Riferimento al sistema di puntamento originale del gioco
    private AiTargetingSystem originalTargetingSystem;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Prendiamo lo script originale per passargli il bersaglio dopo
        originalTargetingSystem = GetComponent<AiTargetingSystem>();
    }

    void Update()
    {
        // Se il nemico è morto o non ne abbiamo uno, cerchiamo
        if (currentTarget == null)
        {
            SearchForEnemy();
        }
        else 
        {
            EngageEnemy();
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
                
                // Diciamo allo script originale CHI è il bersaglio, così inizia a caricare l'arma
                if (originalTargetingSystem != null)
                {
                    originalTargetingSystem.target = currentTarget.gameObject;
                }
                
                Debug.Log(gameObject.name + ": Bersaglio acquisito -> " + currentTarget.name);
                break;
            }
        }
    }

    void EngageEnemy()
    {
        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance > attackRange)
        {
            // Insegui
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);
        }
        else
        {
            // Frena e ruota verso il bersaglio
            agent.isStopped = true;
            
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
            
            // Non dobbiamo scrivere codice per sparare qui! 
            // Il FightScript lo farà in automatico perché in SearchForEnemy gli abbiamo passato il target.
        }
    }

    private void OnDrawGizmosSelected() //solo fuori la scena game vera  e propra
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
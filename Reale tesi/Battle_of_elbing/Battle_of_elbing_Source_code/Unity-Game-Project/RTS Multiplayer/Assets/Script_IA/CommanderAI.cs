using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderAI : MonoBehaviour
{
    [Header("Infrastruttura")]
    public BarracksScript myBarracks;
    public PlayerScript myPlayerStats;
    public Dispatcher globalDispatcher; // Collegamento alla bacheca messaggi

    [Header("Catalogo Unità")]
    public Unit conscriptSO;
    public Unit sniperSO;
    public Unit heavySO;

    [Header("Sensori per ASP")]
    public int currentMoney;
    public int numMessagesInDispatcher; 
    public int currentUnitCount;
    public int maxUnitCount;
    public int baseHealth;
    public int baseHealthPercentage; 

    [Header("Attuatori da ASP")]
    public bool executePurchase = false; // ASP imposta a true per comprare
    public int unitTypeToBuy = 0;        // 1=Conscript, 2=Sniper, 3=Heavy
    public bool EmergencyAction = false;
    public int executeAttackObjectiveId = -1; // NUOVO: -1 = nessun ordine, >=0 = ID obiettivo da attaccare

    private int maxBaseHealth; // Valore di riferimento per calcolare la percentuale

    void Start()
    {
        if (myPlayerStats != null)
        {
            maxBaseHealth = myPlayerStats.hp;
        }
    }

    void Update()
    {
        // 1. AGGIORNAMENTO SENSORI
        if (myPlayerStats != null)
        {
            currentMoney = myPlayerStats.money;
            currentUnitCount = myPlayerStats.population;
            maxUnitCount = myPlayerStats.popCap;
            baseHealth = myPlayerStats.hp;

            if (maxBaseHealth > 0)
            {
                baseHealthPercentage = Mathf.RoundToInt(((float)baseHealth / maxBaseHealth) * 100f);
            }
            else
            {
                maxBaseHealth = baseHealth;
                baseHealthPercentage = 100;
            }
        }

        if (globalDispatcher != null)
        {
            numMessagesInDispatcher = globalDispatcher.activeMessages.Count;
        }

        // 2. ESECUZIONE ORDINI (ATTUATORI)
        if (executePurchase)
        {
            if (unitTypeToBuy != 0)
            {
                PerformPurchase();
            }
            executePurchase = false; 
            unitTypeToBuy = 0;
        }

        if (EmergencyAction)
        {
            TriggerEmergencyLog();
            EmergencyAction = false;
        }

        // NUOVO: Se l'ASP decide un obiettivo d'attacco, lo pubblica sul Dispatcher
        if (executeAttackObjectiveId >= 0 && globalDispatcher != null)
        {
            // Rimuove vecchi ordini d'attacco per pulire la bacheca
            globalDispatcher.activeMessages.RemoveAll(m => m.messageType == "Attack");

            // Invia il nuovo ordine d'attacco globale
            Message attackMsg = new Message
            {
                messageType = "Attack",
                objectiveId = executeAttackObjectiveId
            };
            globalDispatcher.activeMessages.Add(attackMsg);
            
            // Log di conferma
            Debug.Log($"<color=cyan>[COMMANDER-ASP]</color> Inviato ordine d'attacco globale all'obiettivo ID: {executeAttackObjectiveId}");
            
            // Resettiamo l'attuatore per evitare di spammare messaggi identici ad ogni frame se non serve
            executeAttackObjectiveId = -1; 
        }
    }

    void TriggerEmergencyLog()
    {
        Debug.Log("<color=red>[COMMANDER-ASP - TEST]</color> EMERGENZA! La regola 'in_emergency' si è attivata nel file ASP!");
    }

    void PerformPurchase()
    {
        Unit unitToBuy = null;
        switch (unitTypeToBuy)
        {
            case 1: unitToBuy = conscriptSO; break;
            case 2: unitToBuy = heavySO; break;
            case 3: unitToBuy = sniperSO; break;
        }

        if (unitToBuy == null)
        {
            Debug.LogError($"[DEBUG] unitToBuy è NULL! Il valore ricevuto da ASP era: {unitTypeToBuy}");
            return;
        }

        bool canAfford = myPlayerStats.CanAffordUnit(unitToBuy);
        
        if (canAfford)
        {
            Debug.Log($"<color=green>[COMMANDER]</color> Recluto: {unitToBuy.name}");
            myBarracks.RecruitUnit(unitToBuy);
        }
        else
        {
            Debug.LogWarning($"[DEBUG] CANAFFORD FALSE per {unitToBuy.name}. " +
                             $"Costa: {unitToBuy.cost}, Soldi attuali: {myPlayerStats.money}, " +
                             $"Popolazione: {myPlayerStats.population}/{myPlayerStats.popCap}");
        }
    }
}
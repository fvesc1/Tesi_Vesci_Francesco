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

    private int maxBaseHealth; // Valore di riferimento per calcolare la percentuale

    void Start()
    {
        // Rileviamo dinamicamente la salute massima della base all'avvio del gioco
        if (myPlayerStats != null)
        {
            maxBaseHealth = myPlayerStats.hp;
        }
    }

    void Update()
    {
        // 1. AGGIORNAMENTO SENSORI
        // Passiamo i dati all'IA
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
        // Se ASP ha deciso di comprare (executePurchase diventa true)
        if (executePurchase)
        {
            PerformPurchase();
            executePurchase = false; 
        }

        // Se l'interruttore dell'emergenza viene attivato dall'ASP
        if (EmergencyAction)
        {
            TriggerEmergencyLog();
            EmergencyAction = false; // Reset immediato per essere pronti al prossimo trigger
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

        if (unitToBuy != null && myPlayerStats.CanAffordUnit(unitToBuy))
        {
            Debug.Log($"<color=green>[COMMANDER-ASP]</color> Recluto: {unitToBuy.name}");
            myBarracks.RecruitUnit(unitToBuy);
        }
        else
        {
            Debug.LogWarning("[COMMANDER-ASP] Ordine ricevuto ma soldi insufficienti o unità non valida.");
        }
    }
}
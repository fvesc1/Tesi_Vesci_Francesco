using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderAI : MonoBehaviour
{
    [Header("Infrastruttura")]
    public BarracksScript myBarracks;
    public PlayerScript myPlayerStats;

    [Header("Catalogo Unità")]
    public Unit conscriptSO;
    public Unit sniperSO;
    public Unit heavySO;

    [Header("Impostazioni Produzione")]
    public float timeBetweenSpawns = 5f; // todo modificare 
    private float timer = 0f;

    void Update()
    {
        // Il timer avanza in base al tempo reale del gioco
        timer += Time.deltaTime;

        // Quando il timer supera i 5 secondi, è ora di agire
        if (timer >= timeBetweenSpawns)
        {
            timer = 0f; // Resetta il timer per il prossimo giro
            TryBuyUnit();
        }
    }

    void TryBuyUnit()
    {
        // Per ora testiamo solo con la truppa base
        Unit unitToBuy = conscriptSO;

        // Chiediamo al PlayerScript se l'IA ha i soldi necessari
        if (myPlayerStats.CanAffordUnit(unitToBuy))
        {
            Debug.Log("Comandante IA: Soldi sufficienti! Recluto un " + unitToBuy.name);
            // Usiamo la stessa identica funzione che usava il bottone dell'umano!
            myBarracks.RecruitUnit(unitToBuy);
        }
        else
        {
            Debug.Log("Comandante IA: Povertà assoluta. Non posso comprare " + unitToBuy.name);
        }
    }
}
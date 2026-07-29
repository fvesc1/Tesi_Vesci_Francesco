% aI_CommanderSensor_currentMoney(aI_Commander,objectIndex(Index),Value).
% aI_CommanderSensor_numMessagesInDispatcher(aI_Commander,objectIndex(Index),Value).
% aI_CommanderSensor_currentUnitCount(aI_Commander,objectIndex(Index),Value).
% aI_CommanderSensor_maxUnitCount(aI_Commander,objectIndex(Index),Value).
% aI_CommanderSensor_baseHealthPercentage(aI_Commander,objectIndex(Index),Value).
% objective_Sensor_currentOwner(objective_2,objectIndex(Index),Value).
% objective_Sensor_objectiveID(objective_2,objectIndex(Index),Value).
% setOnActuator(aI_CommanderActuator_executePurchase(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .
% setOnActuator(aI_CommanderActuator_unitTypeToBuy(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .
% setOnActuator(aI_CommanderActuator_EmergencyAction(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .
% setOnActuator(aI_CommanderActuator_executeAttackObjectiveId(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% ASP DEDICATO AL RECLUTAMENTO DEI SOLDATI %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% 1. Cosi non si prova a comprare unita se sono gia al massimo
has_population_space :- aI_CommanderSensor_currentUnitCount(aI_Commander, objectIndex(Index), Current), aI_CommanderSensor_maxUnitCount(aI_Commander, objectIndex(_), Max), Current < Max.

% 2. Controllo se ho abbastanza soldi
can_afford_sniper :- aI_CommanderSensor_currentMoney(aI_Commander, objectIndex(Index), Money), Money >= 750.
can_afford_heavy :- aI_CommanderSensor_currentMoney(aI_Commander, objectIndex(Index), Money), Money >= 500.
can_afford_conscript :- aI_CommanderSensor_currentMoney(aI_Commander, objectIndex(Index), Money), Money >= 100.

%%% essenzailmente 2 modalita: normale ed emergenza, emergenza compra conscript velocemente. normale compra sniper heavy conscript in questordine

% STATO DI EMERGENZA (Panic Mode): Se ci sono messaggi nel dispatcher o la base è sotto attacco, 
% spendiamo subito i soldi per difenderci senza aspettare le unità costose.
in_emergency :- aI_CommanderSensor_numMessagesInDispatcher(aI_Commander, objectIndex(Index), N), N > 15.

% CALCOLO SOGLIA CRITICA: Entra in emergenza se gli HP attuali scendono sotto il 20% della vita totale 
in_emergency :- aI_CommanderSensor_baseHealthPercentage(aI_Commander, objectIndex(Index), P), P < 20.

%%%%%% stato normale (non emergenza)

% SCELTA 1: COMPRA SNIPER (Priorità massima assoluta se abbiamo l'economia e spazio, tranne in emergenza totale)
choose_unit(3) :- has_population_space, can_afford_sniper, not in_emergency.

% SCELTA 2: COMPRA HEAVY (Se possiamo permettercelo, c'è spazio e non abbiamo abbastanza soldi per lo Sniper)
choose_unit(2) :- has_population_space, can_afford_heavy, not can_afford_sniper, not in_emergency.

% SCELTA 3: COMPRA CONSCRIPT (Se abbiamo solo i soldi minimi, oppure se siamo in emergency e possiamo permettercelo)
choose_unit(1) :- has_population_space, can_afford_conscript, not can_afford_heavy.
choose_unit(1) :- has_population_space, can_afford_conscript, in_emergency.

%%% attuatori relativi all'acquisto dei soldati
% Inietta il tipo di unità da comprare sull'attuatore (1 = Conscript, 2 = Sniper, 3 = Heavy)
setOnActuator(aI_CommanderActuator_unitTypeToBuy(aI_Commander, objectIndex(Index), Type)) :- objectIndex(aI_CommanderActuator, Index), choose_unit(Type).

% Forza l'esecuzione dell'acquisto impostando il booleano a true se è stata selezionata un'unità
setOnActuator(aI_CommanderActuator_executePurchase(aI_Commander, objectIndex(Index), true)) :- objectIndex(aI_CommanderActuator, Index), choose_unit(_).

% Attiva l'EmergencyAction (true) solo se la regola interna in_emergency è vera
setOnActuator(aI_CommanderActuator_EmergencyAction(aI_Commander, objectIndex(Index), true)) :- objectIndex(aI_CommanderActuator, Index), in_emergency.


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% ASP DEDICATO AL MOVIMENTO DEI SOLDATI %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


% ==========================================================
% LOGICA COMMANDER: SELEZIONE OBIETTIVO PIÙ VICINO (DLV2)
% ==========================================================

% --- 1. FATTI STATICI: Mappatura Nome Stringa -> ID Intero C# e Distanza ---
% objectiveData(Stringa_Unity, ID_Intero_C#, Distanza_Base)
objectiveData("objective_1", 1, 50).
objectiveData("objective_2", 2, 90).
objectiveData("objective_3", 3, 130).
objectiveData("objective_4", 4, 10).
objectiveData("objective_5", 5, 220).
objectiveData("objective_6", 6, 270).
objectiveData("objective_7", 7, 310).
objectiveData("objective_8", 8, 360).


% --- 2. CONTROLLO TRUPPE ---
% Si attiva se abbiamo almeno 1 unità schierata
ready_to_attack :- 
    aI_CommanderSensor_currentUnitCount(aI_Commander, objectIndex(_), Count), 
    Count >= 1.


% --- 3. IDENTIFICAZIONE CANDIDATI NON MIEI ---
% Collega l'ID di Unity (objectIndex) al nome dell'obiettivo e verifica che non sia nostro.
% (Cambia "FRIENDLY" con il nome della tua squadra se diverso, es: "PLAYER")
validObjective(ObjStr, IntId, Dist) :-
    objective_Sensor_objectiveID(objective, objectIndex(ObjIdx), ObjStr),
    objective_Sensor_currentOwner(objective, objectIndex(ObjIdx), Owner),
    Owner != "FRIENDLY",
    objectiveData(ObjStr, IntId, Dist).


% --- 4. CALCOLO DELLA DISTANZA MINIMA (#min) ---
minDistance(MinD) :- 
    validObjective(_, _, _),
    #min { Dist : validObjective(_, _, Dist) } = MinD.


% --- 5. SELEZIONE DEL TARGET (con spareggio sull'ID numerico) ---
closestObjectives(IntId) :- 
    minDistance(MinD),
    validObjective(_, IntId, MinD).

selectedTargetId(BestId) :- 
    closestObjectives(_),
    #min { IntId : closestObjectives(IntId) } = BestId.


% --- 6. INVIO ALL'ATTUATORE ---
setOnActuator(aI_CommanderActuator_executeAttackObjectiveId(aI_Commander, objectIndex(Index), TargetId)) :-
    objectIndex(aI_CommanderActuator, Index),
    selectedTargetId(TargetId),
    ready_to_attack.
% Attuatore per inviare l'ID dell'obiettivo 4 sul Dispatcher tramite C#
%setOnActuator(aI_CommanderActuator_executeAttackObjectiveId(aI_Commander, objectIndex(Index), 4)) :- objectIndex(aI_CommanderActuator, Index), should_attack_target_four.

% Vogliamo ordinare l'attacco all'obiettivo 4 non appena abbiamo almeno 1 unità in vita (o puoi farlo partire sempre)
%should_attack_target_four :- aI_CommanderSensor_currentUnitCount(aI_Commander, objectIndex(_), Count), Count >= 1.

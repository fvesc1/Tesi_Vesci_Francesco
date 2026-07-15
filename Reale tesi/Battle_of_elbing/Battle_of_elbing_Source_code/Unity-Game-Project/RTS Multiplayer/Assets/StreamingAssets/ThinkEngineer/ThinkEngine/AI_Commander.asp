% aI_CommanderSensor_currentMoney(aI_Commander,objectIndex(Index),Value).
% aI_CommanderSensor_numMessagesInDispatcher(aI_Commander,objectIndex(Index),Value).
% aI_CommanderSensor_currentUnitCount(aI_Commander,objectIndex(Index),Value).
% aI_CommanderSensor_maxUnitCount(aI_Commander,objectIndex(Index),Value).
% aI_CommanderSensor_baseHealthPercentage(aI_Commander,objectIndex(Index),Value).
% setOnActuator(aI_CommanderActuator_executePurchase(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .
% setOnActuator(aI_CommanderActuator_unitTypeToBuy(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .
% setOnActuator(aI_CommanderActuator_EmergencyAction(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .

% 1. Cosi non provo a comprare unita se sono gia al massimo
has_population_space :- aI_CommanderSensor_currentUnitCount(aI_Commander, objectIndex(_), Current), aI_CommanderSensor_maxUnitCount(aI_Commander, objectIndex(_), Max), Current < Max.

% 2. Controllo se ho abbastanza soldi
can_afford_sniper :- aI_CommanderSensor_currentMoney(aI_Commander, objectIndex(_), Money), Money >= 750.
can_afford_heavy :- aI_CommanderSensor_currentMoney(aI_Commander, objectIndex(_), Money), Money >= 500.
can_afford_conscript :- aI_CommanderSensor_currentMoney(aI_Commander, objectIndex(_), Money), Money >= 100.

%%% essenzailmente 2 modalita: normale ed emergenza, emergenza compra conscript velocemente. normale compra sniper heavy conscript in questordine

% STATO DI EMERGENZA (Panic Mode): Se ci sono messaggi nel dispatcher o la base è sotto attacco, 
% spendiamo subito i soldi per difenderci senza aspettare le unità costose.
in_emergency :- aI_CommanderSensor_numMessagesInDispatcher(aI_Commander, objectIndex(_), N), N > 15.

% CALCOLO SOGLIA CRITICA: Entra in emergenza se gli HP attuali scendono sotto il 20% della vita totale 
in_emergency :- aI_CommanderSensor_baseHealthPercentage(aI_Commander, objectIndex(_), P), P < 20.

%%%%%% stato normale (non emergenza)

% SCELTA 1: COMPRA SNIPER (Priorità massima assoluta se abbiamo l'economia e spazio, tranne in emergenza totale)
choose_unit(3) :- has_population_space, can_afford_sniper, not in_emergency.

% SCELTA 2: COMPRA HEAVY (Se possiamo permettercelo, c'è spazio e non abbiamo abbastanza soldi per lo Sniper)
choose_unit(2) :- has_population_space, can_afford_heavy, not can_afford_sniper, not in_emergency.

% SCELTA 3: COMPRA CONSCRIPT (Se abbiamo solo i soldi minimi, oppure se siamo in emergency e possiamo permettercelo)
choose_unit(1) :- has_population_space, can_afford_conscript, not can_afford_heavy.
choose_unit(1) :- has_population_space, can_afford_conscript, in_emergency.

%%% attuatori

% Inietta il tipo di unità da comprare sull'attuatore (1 = Conscript, 2 = Sniper, 3 = Heavy)
setOnActuator(aI_CommanderActuator_unitTypeToBuy(aI_Commander, objectIndex(Index), Type)) :- objectIndex(aI_CommanderActuator, Index), choose_unit(Type).

% Forza l'esecuzione dell'acquisto impostando il booleano a true se è stata selezionata un'unità
setOnActuator(aI_CommanderActuator_executePurchase(aI_Commander, objectIndex(Index), true)) :- objectIndex(aI_CommanderActuator, Index), choose_unit(_).

% Attiva l'EmergencyAction (true) solo se la regola interna in_emergency è vera
setOnActuator(aI_CommanderActuator_EmergencyAction(aI_Commander, objectIndex(Index), true)) :- objectIndex(aI_CommanderActuator, Index), in_emergency.
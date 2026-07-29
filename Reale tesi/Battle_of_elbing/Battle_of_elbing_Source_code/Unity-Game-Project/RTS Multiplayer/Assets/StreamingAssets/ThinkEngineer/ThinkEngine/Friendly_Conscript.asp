%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly.
 %Sensors.
%dispatcherSensor_Message_messageType(dispatcher,objectIndex(Index),Index1,Value).
%dispatcherSensor_objectiveId(dispatcher,objectIndex(Index),Index1,Value).
%conscriptSensor_myHealthPercentage(friendlyConscript2,objectIndex(Index),Value).
%conscriptSensor_visibleEnemiesCount(friendlyConscript2,objectIndex(Index),Value).
%conscriptSensor_myUnitId(friendlyConscript2,objectIndex(Index),Value).
%conscriptSensor_isBackupRequested(friendlyConscript2,objectIndex(Index),Value).
%conscriptSensor_amILowHealth(friendlyConscript2,objectIndex(Index),Value).
%conscriptSensor_isAttackOrderPresent(friendlyConscript2,objectIndex(Index),Value).
%conscriptSensor_currentGlobalObjectiveId(friendlyConscript2,objectIndex(Index),Value).
%Actuators:
%setOnActuator(conscriptActuator_hasAspOrder(friendlyConscript2,objectIndex(Index),Value)) :-objectIndex(conscriptActuator, Index), .
%setOnActuator(conscriptActuator_aspTargetObjectiveId(friendlyConscript2,objectIndex(Index),Value)) :-objectIndex(conscriptActuator, Index), .

% 1. Estraiamo i valori dai sensori. 
% Usando "_" ignoriamo l'ID del sensore, evitando il mismatch con l'attuatore!
orderStatus(Unit, true)  :- conscriptSensor_isAttackOrderPresent(Unit, objectIndex(_), true).
orderStatus(Unit, false) :- conscriptSensor_isAttackOrderPresent(Unit, objectIndex(_), false).

targetId(Unit, TargetId) :- conscriptSensor_currentGlobalObjectiveId(Unit, objectIndex(_), TargetId).

% 2. Impostiamo gli attuatori agganciandoli ESCLUSIVAMENTE al loro indice (I)
% Proprio come nel tuo esempio: setOnActuator(...) :- objectIndex(actuator, I), status(X).

% Applica lo stato dell'ordine (true/false) all'attuatore
setOnActuator(conscriptActuator_hasAspOrder(Unit, objectIndex(BrainID), Status)) :- 
    currentBrainID(BrainID),
    objectIndex(conscriptActuator, BrainID), 
    orderStatus(Unit, Status).

% Applica il Target ID all'attuatore solo se l'ordine è true
setOnActuator(conscriptActuator_aspTargetObjectiveId(Unit, objectIndex(BrainID), TargetId)) :- 
    currentBrainID(BrainID),
    objectIndex(conscriptActuator, BrainID), 
    orderStatus(Unit, true),
    targetId(Unit, TargetId).
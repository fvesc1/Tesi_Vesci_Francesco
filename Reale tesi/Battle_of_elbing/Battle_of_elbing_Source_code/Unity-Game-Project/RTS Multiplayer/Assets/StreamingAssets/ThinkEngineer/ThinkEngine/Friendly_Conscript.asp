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

% 1. Estrazione del sensore di difesa (Legato all'ID unico del soldato)
inDefense(Unit, BrainID, true)  :- conscriptSensor_isDefending(Unit, objectIndex(BrainID), true).
inDefense(Unit, BrainID, false) :- conscriptSensor_isDefending(Unit, objectIndex(BrainID), false).

% 2. Estrazione dello stato dell'ordine (Legato all'ID)
orderStatus(Unit, BrainID, true) :- 
    conscriptSensor_isAttackOrderPresent(Unit, objectIndex(BrainID), true),
    inDefense(Unit, BrainID, false).

orderStatus(Unit, BrainID, false) :- 
    conscriptSensor_isAttackOrderPresent(Unit, objectIndex(BrainID), false).

orderStatus(Unit, BrainID, false) :- 
    inDefense(Unit, BrainID, true).

targetId(Unit, BrainID, TargetId) :- 
    conscriptSensor_currentGlobalObjectiveId(Unit, objectIndex(BrainID), TargetId).

% 3. Applicazione agli attuatori (solo per il currentBrainID)
setOnActuator(conscriptActuator_hasAspOrder(Unit, objectIndex(BrainID), Status)) :- 
    currentBrainID(BrainID),
    objectIndex(conscriptActuator, BrainID), 
    orderStatus(Unit, BrainID, Status).

setOnActuator(conscriptActuator_aspTargetObjectiveId(Unit, objectIndex(BrainID), TargetId)) :- 
    currentBrainID(BrainID),
    objectIndex(conscriptActuator, BrainID), 
    orderStatus(Unit, BrainID, true),
    targetId(Unit, BrainID, TargetId).
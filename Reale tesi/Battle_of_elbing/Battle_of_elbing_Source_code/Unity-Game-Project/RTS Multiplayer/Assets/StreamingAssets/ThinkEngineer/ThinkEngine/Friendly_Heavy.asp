%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly.
 %Sensors.
%dispatcherSensor_Message_messageType(dispatcher,objectIndex(Index),Index1,Value).
%dispatcherSensor_objectiveId(dispatcher,objectIndex(Index),Index1,Value).
%heavySensor_visibleEnemiesCount(friendlyHeavy,objectIndex(Index),Value).
%heavySensor_myHealthPercentage(friendlyHeavy,objectIndex(Index),Value).
%heavySensor_myUnitId(friendlyHeavy,objectIndex(Index),Value).
%heavySensor_isBackupRequested(friendlyHeavy,objectIndex(Index),Value).
%heavySensor_amILowHealth(friendlyHeavy,objectIndex(Index),Value).
%heavySensor_isAttackOrderPresent(friendlyHeavy,objectIndex(Index),Value).
%heavySensor_currentGlobalObjectiveId(friendlyHeavy,objectIndex(Index),Value).
%heavySensor_isDefending(friendlyHeavy,objectIndex(Index),Value).
%Actuators:
%setOnActuator(heavyActuator_hasAspOrder(friendlyHeavy,objectIndex(Index),Value)) :-objectIndex(heavyActuator, Index), .
%setOnActuator(heavyActuator_aspTargetObjectiveId(friendlyHeavy,objectIndex(Index),Value)) :-objectIndex(heavyActuator, Index), .

% 1. Estrazione del sensore di difesa
heavyInDefense(Unit, BrainID, true)  :- heavySensor_isDefending(Unit, objectIndex(BrainID), true).
heavyInDefense(Unit, BrainID, false) :- heavySensor_isDefending(Unit, objectIndex(BrainID), false).

% 2. Estrazione dello stato dell'ordine
heavyOrderStatus(Unit, BrainID, true) :- 
    heavySensor_isAttackOrderPresent(Unit, objectIndex(BrainID), true),
    heavyInDefense(Unit, BrainID, false).

heavyOrderStatus(Unit, BrainID, false) :- 
    heavySensor_isAttackOrderPresent(Unit, objectIndex(BrainID), false).

heavyOrderStatus(Unit, BrainID, false) :- 
    heavyInDefense(Unit, BrainID, true).

heavyTargetId(Unit, BrainID, TargetId) :- 
    heavySensor_currentGlobalObjectiveId(Unit, objectIndex(BrainID), TargetId).

% 3. Applicazione agli attuatori
setOnActuator(heavyActuator_hasAspOrder(Unit, objectIndex(BrainID), Status)) :- 
    currentBrainID(BrainID),
    objectIndex(heavyActuator, BrainID), 
    heavyOrderStatus(Unit, BrainID, Status).

setOnActuator(heavyActuator_aspTargetObjectiveId(Unit, objectIndex(BrainID), TargetId)) :- 
    currentBrainID(BrainID),
    objectIndex(heavyActuator, BrainID), 
    heavyOrderStatus(Unit, BrainID, true),
    heavyTargetId(Unit, BrainID, TargetId).
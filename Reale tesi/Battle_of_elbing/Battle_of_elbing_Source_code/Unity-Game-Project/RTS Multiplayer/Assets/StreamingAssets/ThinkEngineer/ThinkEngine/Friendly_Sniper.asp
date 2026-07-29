%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly.
 %Sensors.
%dispatcherSensor_Message_messageType(dispatcher,objectIndex(Index),Index1,Value).
%dispatcherSensor_objectiveId(dispatcher,objectIndex(Index),Index1,Value).
%sniperSensor_myHealthPercentage(friendlySniper,objectIndex(Index),Value).
%sniperSensor_visibleEnemiesCount(friendlySniper,objectIndex(Index),Value).
%sniperSensor_myUnitId(friendlySniper,objectIndex(Index),Value).
%sniperSensor_isBackupRequested(friendlySniper,objectIndex(Index),Value).
%sniperSensor_amILowHealth(friendlySniper,objectIndex(Index),Value).
%sniperSensor_isAttackOrderPresent(friendlySniper,objectIndex(Index),Value).
%sniperSensor_currentGlobalObjectiveId(friendlySniper,objectIndex(Index),Value).
%sniperSensor_isDefending(friendlySniper,objectIndex(Index),Value).
%Actuators:
%setOnActuator(sniperActuator_hasAspOrder(friendlySniper,objectIndex(Index),Value)) :-objectIndex(sniperActuator, Index), .
%setOnActuator(sniperActuator_aspTargetObjectiveId(friendlySniper,objectIndex(Index),Value)) :-objectIndex(sniperActuator, Index), .

% 1. Estrazione del sensore di difesa
sniperInDefense(Unit, BrainID, true)  :- sniperSensor_isDefending(Unit, objectIndex(BrainID), true).
sniperInDefense(Unit, BrainID, false) :- sniperSensor_isDefending(Unit, objectIndex(BrainID), false).

% 2. Estrazione dello stato dell'ordine 
sniperOrderStatus(Unit, BrainID, true) :- 
    sniperSensor_isAttackOrderPresent(Unit, objectIndex(BrainID), true),
    sniperInDefense(Unit, BrainID, false).

sniperOrderStatus(Unit, BrainID, false) :- 
    sniperSensor_isAttackOrderPresent(Unit, objectIndex(BrainID), false).

sniperOrderStatus(Unit, BrainID, false) :- 
    sniperInDefense(Unit, BrainID, true).

sniperTargetId(Unit, BrainID, TargetId) :- 
    sniperSensor_currentGlobalObjectiveId(Unit, objectIndex(BrainID), TargetId).

% 3. Applicazione agli attuatori
setOnActuator(sniperActuator_hasAspOrder(Unit, objectIndex(BrainID), Status)) :- 
    currentBrainID(BrainID),
    objectIndex(sniperActuator, BrainID), 
    sniperOrderStatus(Unit, BrainID, Status).

setOnActuator(sniperActuator_aspTargetObjectiveId(Unit, objectIndex(BrainID), TargetId)) :- 
    currentBrainID(BrainID),
    objectIndex(sniperActuator, BrainID), 
    sniperOrderStatus(Unit, BrainID, true),
    sniperTargetId(Unit, BrainID, TargetId).
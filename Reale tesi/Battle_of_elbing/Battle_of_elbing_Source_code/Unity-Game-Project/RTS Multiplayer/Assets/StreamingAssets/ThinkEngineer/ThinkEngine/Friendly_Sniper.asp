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
%Actuators:
%setOnActuator(sniperActuator_hasAspOrder(friendlySniper,objectIndex(Index),Value)) :-objectIndex(sniperActuator, Index), .
%setOnActuator(sniperActuator_aspTargetObjectiveId(friendlySniper,objectIndex(Index),Value)) :-objectIndex(sniperActuator, Index), .
% ==========================================
% LOGICA DI RAGIONAMENTO: SNIPER
% ==========================================

% ==========================================
% LOGICA SNIPER (Multi-Istanza Runtime)
% ==========================================

% ==========================================
% LOGICA SNIPER (Pattern con Indice Dinamico)
% ==========================================

% 1. Estrazione valori dai sensori dello Sniper (ignorando l'ID sensore con "_")
% ==========================================
% LOGICA SNIPER (con currentBrainID)
% ==========================================

% 1. Estrazione dai sensori
sniperOrderStatus(Unit, true)  :- sniperSensor_isAttackOrderPresent(Unit, objectIndex(_), true).
sniperOrderStatus(Unit, false) :- sniperSensor_isAttackOrderPresent(Unit, objectIndex(_), false).

sniperTargetId(Unit, TargetId) :- sniperSensor_currentGlobalObjectiveId(Unit, objectIndex(_), TargetId).

% 2. Applicazione agli attuatori con currentBrainID
setOnActuator(sniperActuator_hasAspOrder(Unit, objectIndex(BrainID), Status)) :- 
    currentBrainID(BrainID),
    objectIndex(sniperActuator, BrainID), 
    sniperOrderStatus(Unit, Status).

setOnActuator(sniperActuator_aspTargetObjectiveId(Unit, objectIndex(BrainID), TargetId)) :- 
    currentBrainID(BrainID),
    objectIndex(sniperActuator, BrainID), 
    sniperOrderStatus(Unit, true),
    sniperTargetId(Unit, TargetId).

#show setOnActuator/1.
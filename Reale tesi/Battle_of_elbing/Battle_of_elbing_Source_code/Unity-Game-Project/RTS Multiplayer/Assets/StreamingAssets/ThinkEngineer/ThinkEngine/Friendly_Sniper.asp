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

% 1. Determina se questa specifica unità ha un ordine di attacco valido
has_attack_order(Unit, TargetId) :- sniperSensor_isAttackOrderPresent(Unit, objectIndex(Index), true), sniperSensor_currentGlobalObjectiveId(Unit, objectIndex(Index), TargetId).

% ==========================================
% ATTUATORI
% ==========================================

% 2. Se c'è l'ordine, imposta l'ID del bersaglio
setOnActuator(sniperActuator_aspTargetObjectiveId(Unit, objectIndex(Index), TargetId)) :- objectIndex(sniperActuator, Index), has_attack_order(Unit, TargetId).

% 3. Se c'è l'ordine, accendi l'attuatore di movimento (true)
setOnActuator(sniperActuator_hasAspOrder(Unit, objectIndex(Index), true)) :- objectIndex(sniperActuator, Index), has_attack_order(Unit, _).

% 4. Se NON c'è l'ordine, spegni l'attuatore di movimento (false).
setOnActuator(sniperActuator_hasAspOrder(Unit, objectIndex(Index), false)) :- objectIndex(sniperActuator, Index), sniperSensor_isAttackOrderPresent(Unit, _, _), not has_attack_order(Unit, _).
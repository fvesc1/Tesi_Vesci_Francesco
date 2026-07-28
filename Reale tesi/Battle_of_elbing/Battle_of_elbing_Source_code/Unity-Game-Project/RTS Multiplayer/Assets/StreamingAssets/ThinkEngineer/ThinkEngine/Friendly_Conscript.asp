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
% ==========================================
% LOGICA CONSCRIPT (Fanteria Bilanciata)
% ==========================================

% 1. L'ordine è valido solo se c'è l'ordine E la salute NON è critica
has_attack_order(Unit, TargetId) :- conscriptSensor_isAttackOrderPresent(Unit, _, true), conscriptSensor_currentGlobalObjectiveId(Unit, _, TargetId), conscriptSensor_amILowHealth(Unit, _, false).

% ==========================================
% ATTUATORI
% ==========================================

% Imposta l'obiettivo se l'ordine è valido
setOnActuator(conscriptActuator_aspTargetObjectiveId(Unit, objectIndex(Index), TargetId)) :- objectIndex(conscriptActuator, Index), has_attack_order(Unit, TargetId).

% Attiva il movimento se l'ordine è valido
setOnActuator(conscriptActuator_hasAspOrder(Unit, objectIndex(Index), true)) :- objectIndex(conscriptActuator, Index), has_attack_order(Unit, _).

% Disattiva l'ordine se non c'è l'ordine o se la salute è troppo bassa
setOnActuator(conscriptActuator_hasAspOrder(Unit, objectIndex(Index), false)) :- objectIndex(conscriptActuator, Index), conscriptSensor_isAttackOrderPresent(Unit, _, _), not has_attack_order(Unit, _).
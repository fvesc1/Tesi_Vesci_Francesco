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
%Actuators:
%setOnActuator(heavyActuator_hasAspOrder(friendlyHeavy,objectIndex(Index),Value)) :-objectIndex(heavyActuator, Index), .
%setOnActuator(heavyActuator_aspTargetObjectiveId(friendlyHeavy,objectIndex(Index),Value)) :-objectIndex(heavyActuator, Index), .

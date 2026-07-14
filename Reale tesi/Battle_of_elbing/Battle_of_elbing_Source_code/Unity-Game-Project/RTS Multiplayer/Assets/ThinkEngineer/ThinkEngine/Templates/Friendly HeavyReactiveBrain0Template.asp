%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly.
 %Sensors.
%dispatcherSensor_Message_messageType(dispatcher,objectIndex(Index),Index1,Value).
%dispatcherSensor_objectiveId(dispatcher,objectIndex(Index),Index1,Value).
%heavySensor_visibleEnemiesCount(friendlyHeavy,objectIndex(Index),Value).
%heavySensor_myHealthPercentage(friendlyHeavy,objectIndex(Index),Value).
%Actuators:
%setOnActuator(heavyActuator_hasAspOrder(friendlyHeavy,objectIndex(Index),Value)) :-objectIndex(heavyActuator, Index), .
%setOnActuator(heavyActuator_aspTargetX(friendlyHeavy,objectIndex(Index),Value)) :-objectIndex(heavyActuator, Index), .
%setOnActuator(heavyActuator_aspTargetY(friendlyHeavy,objectIndex(Index),Value)) :-objectIndex(heavyActuator, Index), .
%setOnActuator(heavyActuator_aspTargetZ(friendlyHeavy,objectIndex(Index),Value)) :-objectIndex(heavyActuator, Index), .

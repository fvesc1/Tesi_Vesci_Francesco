%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly.
 %Sensors.
%dispatcherSensor_Message_messageType(dispatcher,objectIndex(Index),Index1,Value).
%dispatcherSensor_objectiveId(dispatcher,objectIndex(Index),Index1,Value).
%sniperSensor_myHealthPercentage(friendlySniper,objectIndex(Index),Value).
%sniperSensor_visibleEnemiesCount(friendlySniper,objectIndex(Index),Value).
%Actuators:
setOnActuator(sniperActuator_hasAspOrder(friendlySniper,objectIndex(Index),Value)) :-objectIndex(sniperActuator, Index), .
setOnActuator(sniperActuator_aspTargetX(friendlySniper,objectIndex(Index),Value)) :-objectIndex(sniperActuator, Index), .
setOnActuator(sniperActuator_aspTargetY(friendlySniper,objectIndex(Index),Value)) :-objectIndex(sniperActuator, Index), .
setOnActuator(sniperActuator_aspTargetZ(friendlySniper,objectIndex(Index),Value)) :-objectIndex(sniperActuator, Index), .

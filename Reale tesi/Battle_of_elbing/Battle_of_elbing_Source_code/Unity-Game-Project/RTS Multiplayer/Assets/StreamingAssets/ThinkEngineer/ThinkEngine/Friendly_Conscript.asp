%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly.
 %Sensors.
%dispatcherSensor_Message_messageType(dispatcher,objectIndex(Index),Index1,Value).
%dispatcherSensor_objectiveId(dispatcher,objectIndex(Index),Index1,Value).
%conscriptSensor_myHealthPercentage(friendlyConscript,objectIndex(Index),Value).
%conscriptSensor_visibleEnemiesCount(friendlyConscript,objectIndex(Index),Value).
%Actuators:
%setOnActuator(conscriptActuator_hasAspOrder(friendlyConscript,objectIndex(Index),Value)) :-objectIndex(conscriptActuator, Index), .
%setOnActuator(conscriptActuator_aspTargetX(friendlyConscript,objectIndex(Index),Value)) :-objectIndex(conscriptActuator, Index), .
%setOnActuator(conscriptActuator_aspTargetY(friendlyConscript,objectIndex(Index),Value)) :-objectIndex(conscriptActuator, Index), .
%setOnActuator(conscriptActuator_aspTargetZ(friendlyConscript,objectIndex(Index),Value)) :-objectIndex(conscriptActuator, Index), .

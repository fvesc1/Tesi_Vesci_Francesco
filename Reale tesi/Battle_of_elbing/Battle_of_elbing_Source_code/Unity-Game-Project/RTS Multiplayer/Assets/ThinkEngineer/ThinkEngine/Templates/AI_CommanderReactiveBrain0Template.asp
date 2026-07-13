%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly.
 %Sensors.
%aI_CommanderSensor_currentMoney(aI_Commander,objectIndex(Index),Value).
%aI_CommanderSensor_numMessagesInDispatcher(aI_Commander,objectIndex(Index),Value).
%aI_CommanderSensor_currentUnitCount(aI_Commander,objectIndex(Index),Value).
%aI_CommanderSensor_maxUnitCount(aI_Commander,objectIndex(Index),Value).
%aI_CommanderSensor_baseHealth(aI_Commander,objectIndex(Index),Value).
%Actuators:
setOnActuator(aI_CommanderActuator_executePurchase(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .
setOnActuator(aI_CommanderActuator_unitTypeToBuy(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .
setOnActuator(aI_CommanderActuator_EmergencyAction(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .

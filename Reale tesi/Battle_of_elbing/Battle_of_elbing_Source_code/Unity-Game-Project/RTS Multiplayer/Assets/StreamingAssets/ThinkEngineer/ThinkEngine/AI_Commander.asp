%aI_CommanderSensor_currentMoney(aI_Commander,objectIndex(Index),Value).
%aI_CommanderSensor_numMessagesInDispatcher(aI_Commander,objectIndex(Index),Value).
%aI_CommanderSensor_currentUnitCount(aI_Commander,objectIndex(Index),Value).
%aI_CommanderSensor_maxUnitCount(aI_Commander,objectIndex(Index),Value).
%aI_CommanderSensor_baseHealth(aI_Commander,objectIndex(Index),Value).
%setOnActuator(aI_CommanderActuator_executePurchase(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .
%setOnActuator(aI_CommanderActuator_unitTypeToBuy(aI_Commander,objectIndex(Index),Value)) :-objectIndex(aI_CommanderActuator, Index), .
compra(1) :- objectIndex(aI_CommanderActuator, Index), aI_CommanderSensor_currentMoney(aI_Commander, objectIndex(Index), M), M > 99.
setOnActuator(aI_CommanderActuator_unitTypeToBuy(aI_Commander,objectIndex(Index), 1)) :-objectIndex(aI_CommanderActuator, Index), 
    aI_CommanderSensor_currentMoney(aI_Commander, objectIndex(Index), M), M > 99.
setOnActuator(aI_CommanderActuator_executePurchase(aI_Commander,objectIndex(Index), true)) :-objectIndex(aI_CommanderActuator, Index), compra(1). 

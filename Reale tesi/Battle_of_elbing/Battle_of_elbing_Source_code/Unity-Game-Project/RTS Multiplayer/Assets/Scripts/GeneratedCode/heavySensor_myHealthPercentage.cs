using UnityEngine;
using System;
using System.Collections.Generic;
using ThinkEngine.Mappers;
using static ThinkEngine.Mappers.OperationContainer;
namespace ThinkEngine
{
	public class heavySensor_myHealthPercentage : Sensor
	{
		private int counter;
		private object specificValue;
		private Operation operation;
		private BasicTypeMapper mapper;
		private List<float> values = new List<float>();
		public override void Initialize(SensorConfiguration sensorConfiguration)
		{
			this.gameObject = sensorConfiguration.gameObject;
			ready = true;
			int index = gameObject.GetInstanceID();
			mapper = (BasicTypeMapper)MapperManager.GetMapper(typeof(float));
			operation = mapper.OperationList()[0];
			counter = 0;
			mappingTemplate = "heavySensor_myHealthPercentage(friendlyHeavy,objectIndex("+index+"),{0})." + Environment.NewLine;
		}
		public override void Destroy()
		{
		}
		public override void Update()
		{
			if(!ready)
			{
				return;
			}
			if(!invariant || first)
			{
				first = false;
				SoldierBrain SoldierBrain_1 = gameObject.GetComponent<SoldierBrain>();
				if(SoldierBrain_1 == null)
				{
					values.Clear();
					return;
				}
				if(SoldierBrain_1 == null)
				{
					values.Clear();
					return;
				}
				float myHealthPercentage_2 = SoldierBrain_1.myHealthPercentage;
				if (values.Count == 1)
				{
					values.RemoveAt(0);
				}
				values.Add(myHealthPercentage_2);
			}
		}
		public override string Map()
		{
			object operationResult = operation(values, specificValue, counter);
			if(operationResult != null)
			{
				return string.Format(mappingTemplate, BasicTypeMapper.GetMapper(operationResult.GetType()).BasicMap(operationResult));
			}
			else
			{
				return "";
			}
		}
	}
}
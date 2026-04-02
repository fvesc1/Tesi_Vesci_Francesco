using UnityEngine;
using System;
using System.Collections.Generic;
using ThinkEngine.Mappers;
using static ThinkEngine.Mappers.OperationContainer;
namespace ThinkEngine
{
	public class objective_Sensor_currentOwner : Sensor
	{
		private int counter;
		private object specificValue;
		private Operation operation;
		private BasicTypeMapper mapper;
		private List<string> values = new List<string>();
		public override void Initialize(SensorConfiguration sensorConfiguration)
		{
			this.gameObject = sensorConfiguration.gameObject;
			ready = true;
			int index = gameObject.GetInstanceID();
			mapper = (BasicTypeMapper)MapperManager.GetMapper(typeof(string));
			operation = mapper.OperationList()[0];
			counter = 0;
			mappingTemplate = "objective_Sensor_currentOwner(objective_1,objectIndex("+index+"),{0})." + Environment.NewLine;
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
				ObjectiveSensor ObjectiveSensor_1 = gameObject.GetComponent<ObjectiveSensor>();
				if(ObjectiveSensor_1 == null)
				{
					values.Clear();
					return;
				}
				if(ObjectiveSensor_1 == null)
				{
					values.Clear();
					return;
				}
				string currentOwner_2 = ObjectiveSensor_1.currentOwner;
				if(currentOwner_2 == null)
				{
					values.Clear();
					return;
				}
				if (values.Count == 1)
				{
					values.RemoveAt(0);
				}
				values.Add(currentOwner_2);
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
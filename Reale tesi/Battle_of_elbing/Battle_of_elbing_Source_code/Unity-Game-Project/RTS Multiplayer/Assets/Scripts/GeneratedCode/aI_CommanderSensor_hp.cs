using UnityEngine;
using System;
using System.Collections.Generic;
using ThinkEngine.Mappers;
using static ThinkEngine.Mappers.OperationContainer;
namespace ThinkEngine
{
	public class aI_CommanderSensor_hp : Sensor
	{
		private int counter;
		private object specificValue;
		private Operation operation;
		private BasicTypeMapper mapper;
		private List<int> values = new List<int>();
		public override void Initialize(SensorConfiguration sensorConfiguration)
		{
			this.gameObject = sensorConfiguration.gameObject;
			ready = true;
			int index = gameObject.GetInstanceID();
			mapper = (BasicTypeMapper)MapperManager.GetMapper(typeof(int));
			operation = mapper.OperationList()[0];
			counter = 0;
			mappingTemplate = "aI_CommanderSensor_hp(aI_Commander,objectIndex("+index+"),{0})." + Environment.NewLine;
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
				CommanderAI CommanderAI_1 = gameObject.GetComponent<CommanderAI>();
				if(CommanderAI_1 == null)
				{
					values.Clear();
					return;
				}
				if(CommanderAI_1 == null)
				{
					values.Clear();
					return;
				}
				PlayerScript myPlayerStats_2 = CommanderAI_1.myPlayerStats;
				if(myPlayerStats_2 == null)
				{
					values.Clear();
					return;
				}
				if(myPlayerStats_2 == null)
				{
					values.Clear();
					return;
				}
				int hp_3 = myPlayerStats_2.hp;
				if (values.Count == 1)
				{
					values.RemoveAt(0);
				}
				values.Add(hp_3);
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
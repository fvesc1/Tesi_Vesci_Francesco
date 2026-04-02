using UnityEngine;
using System;
using System.Collections.Generic;
using ThinkEngine.Mappers;
using static ThinkEngine.Mappers.OperationContainer;
namespace ThinkEngine
{
	public class sniperSensor_visibleEnemiesCount : Sensor
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
			mappingTemplate = "sniperSensor_visibleEnemiesCount(friendlySniper,objectIndex("+index+"),{0})." + Environment.NewLine;
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
				int visibleEnemiesCount_2 = SoldierBrain_1.visibleEnemiesCount;
				if (values.Count == 1)
				{
					values.RemoveAt(0);
				}
				values.Add(visibleEnemiesCount_2);
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
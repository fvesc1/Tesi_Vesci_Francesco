using UnityEngine;
using System;
using System.Collections.Generic;
using ThinkEngine.Mappers;
using static ThinkEngine.Mappers.OperationContainer;
namespace ThinkEngine
{
	public class dispatcherSensor_messageType : Sensor
	{
		private int counter;
		private object specificValue;
		private Operation operation;
		private BasicTypeMapper mapper;
		private List<List<string>> values = new List<List<string>>();
		public override void Initialize(SensorConfiguration sensorConfiguration)
		{
			this.gameObject = sensorConfiguration.gameObject;
			ready = true;
			int index = gameObject.GetInstanceID();
			mapper = (BasicTypeMapper)MapperManager.GetMapper(typeof(string));
			operation = mapper.OperationList()[0];
			counter = 0;
			mappingTemplate = "dispatcherSensor_messageType(dispatcher,objectIndex("+index+"),{1},{0})." + Environment.NewLine;
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
				Dispatcher Dispatcher_1 = gameObject.GetComponent<Dispatcher>();
				if(Dispatcher_1 == null)
				{
					values.Clear();
					return;
				}
				List<Message> activeMessages_2 = Dispatcher_1.activeMessages;
				if(activeMessages_2 == null)
				{
					values.Clear();
					return;
				}
				else if(activeMessages_2.Count > values.Count)
				{
					for(int i = values.Count; i < activeMessages_2.Count; i++)
					{
						values.Add(new List<string>());
					}
				}
				else if(activeMessages_2.Count < values.Count)
				{
					for(int i = activeMessages_2.Count; i < values.Count; i++)
					{
						values.RemoveAt(values.Count - 1);
					}
				}
				for(int i_2 = 0; i_2 < activeMessages_2.Count; i_2++)
				{
					if(activeMessages_2[i_2] == null)
					{
						values[i_2].Clear();
						continue;
					}
					string messageType_3 = activeMessages_2[i_2].messageType;
					if(messageType_3 == null)
					{
						values[i_2].Clear();
						continue;
					}
					if (values[i_2].Count == 1)
					{
						values[i_2].RemoveAt(0);
					}
					values[i_2].Add(messageType_3);
				}
			}
		}
		public override string Map()
		{
			string mapping = string.Empty;
			for(int i0 = 0; i0 < values.Count; i0++)
			{
				object operationResult = operation(values[i0], specificValue, counter);
				if(operationResult != null)
				{
					mapping = string.Concat(mapping, string.Format(mappingTemplate, BasicTypeMapper.GetMapper(operationResult.GetType()).BasicMap(operationResult),i0));
				}
				else
				{
					mapping = string.Concat(mapping, string.Format("{0}", Environment.NewLine));
				}
			}
			return mapping;
		}
	}
}
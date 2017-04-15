using System;
using System.Collections.Generic;

namespace Assets.MobaTools.TriggerPlugin.Scripts
{
	public class TriggerManager2
	{
		private static TriggerManager2 instance;

		private Dictionary<ETriggerType2, Dictionary<int, Dictionary<string, TriggerEvent2>>> tMap;

		public static TriggerManager2 Instance
		{
			get
			{
				if (TriggerManager2.instance == null)
				{
					TriggerManager2.instance = new TriggerManager2();
				}
				return TriggerManager2.instance;
			}
		}

		protected TriggerManager2()
		{
			this.tMap = new Dictionary<ETriggerType2, Dictionary<int, Dictionary<string, TriggerEvent2>>>();
		}

		public void AddListener(TriggerEvent2 trigger)
		{
			if (trigger == null)
			{
				return;
			}
			ETriggerType2 typeID = trigger.TypeID;
			int eventID = trigger.EventID;
			if (!this.tMap.ContainsKey(typeID))
			{
				this.tMap.Add(typeID, new Dictionary<int, Dictionary<string, TriggerEvent2>>());
				if (!this.tMap.ContainsKey(typeID))
				{
					throw new InsufficientMemoryException();
				}
			}
			if (!this.tMap[typeID].ContainsKey(eventID))
			{
				this.tMap[typeID].Add(eventID, new Dictionary<string, TriggerEvent2>());
				if (!this.tMap[typeID].ContainsKey(eventID))
				{
					throw new InsufficientMemoryException();
				}
			}
			if (!this.tMap[typeID][eventID].ContainsKey(trigger.TriggerID))
			{
				this.tMap[typeID][eventID].Add(trigger.TriggerID, trigger);
			}
		}

		public void RemoveListner(ETriggerType2 typeID)
		{
			if (this.tMap.ContainsKey(typeID))
			{
				this.tMap.Remove(typeID);
			}
		}

		public void RemoveListner(ETriggerType2 typeID, int eventID)
		{
			if (this.tMap.ContainsKey(typeID) && this.tMap[typeID].ContainsKey(eventID))
			{
				this.tMap[typeID].Remove(eventID);
			}
		}

		public void RemoveListner(TriggerEvent2 trigger)
		{
			if (trigger == null)
			{
				return;
			}
			ETriggerType2 typeID = trigger.TypeID;
			int eventID = trigger.EventID;
			string triggerID = trigger.TriggerID;
			if (this.tMap.ContainsKey(typeID) && this.tMap[typeID].ContainsKey(eventID) && this.tMap[typeID][eventID].ContainsKey(triggerID))
			{
				this.tMap[typeID][eventID].Remove(triggerID);
			}
		}

		public void Trigger2(ITriggerDoActionParam param)
		{
			if (param == null)
			{
				return;
			}
			ETriggerType2 typeID = param.TypeID;
			int eventID = param.EventID;
			if (this.tMap.ContainsKey(typeID) && this.tMap[typeID].ContainsKey(eventID))
			{
				Dictionary<string, TriggerEvent2> dictionary = this.tMap[typeID][eventID];
				foreach (KeyValuePair<string, TriggerEvent2> current in dictionary)
				{
					if (current.Value.CheckConditions(param))
					{
						current.Value.DoActions(param);
					}
				}
			}
		}

		public static TriggerEvent2 CreateTriggerEvent2(ITriggerCreatorParam param)
		{
			TriggerEvent2 result = null;
			try
			{
				Type type = Type.GetType("Assets.MobaTools.TriggerPlugin.Scripts." + param.TypeID.ToString());
				object obj = Activator.CreateInstance(type, new object[]
				{
					param
				});
				if (obj != null)
				{
					result = (obj as TriggerEvent2);
				}
			}
			catch (Exception var_3_45)
			{
				result = null;
			}
			return result;
		}

		public static string assign_trigger_id()
		{
			return Guid.NewGuid().ToString();
		}
	}
}

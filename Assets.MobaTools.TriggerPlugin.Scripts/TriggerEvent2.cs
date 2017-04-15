using System;
using System.Collections.Generic;

namespace Assets.MobaTools.TriggerPlugin.Scripts
{
	public class TriggerEvent2
	{
		private string trigger_id;

		private ETriggerType2 type_id;

		private int event_id;

		private List<TriggerCondition<ITriggerDoActionParam>> func_conditions;

		private Callback<ITriggerDoActionParam> func_actions;

		public ETriggerType2 TypeID
		{
			get
			{
				return this.type_id;
			}
		}

		public int EventID
		{
			get
			{
				return this.event_id;
			}
		}

		public string TriggerID
		{
			get
			{
				return this.trigger_id;
			}
		}

		public TriggerEvent2(ITriggerCreatorParam param)
		{
			if (param != null)
			{
				this.type_id = param.TypeID;
				this.event_id = param.EventID;
				this.trigger_id = param.TriggerID;
				this.func_conditions = param.Func_conditions;
				this.func_actions = param.Func_actions;
			}
		}

		public bool CheckConditions(ITriggerDoActionParam param = null)
		{
			bool flag = true;
			if (this.func_conditions != null)
			{
				for (int i = 0; i < this.func_conditions.Count; i++)
				{
					if (this.func_conditions[i] != null)
					{
						flag &= this.func_conditions[i](param);
					}
				}
			}
			return flag;
		}

		public void DoActions(ITriggerDoActionParam param = null)
		{
			if (this.func_actions != null)
			{
				this.func_actions(param);
			}
		}
	}
}

using System;
using System.Collections.Generic;

public class VTrigger
{
	public List<TriggerAction> func_actions = new List<TriggerAction>();

	public List<TriggerCondition> func_conditions = new List<TriggerCondition>();

	public int trigger_event;

	public int trigger_id;

	public int trigger_type;

	public VTrigger(int id)
	{
		this.trigger_id = id;
	}

	public bool CheckConditions()
	{
		bool flag = true;
		for (int i = 0; i < this.func_conditions.Count; i++)
		{
			if (this.func_conditions[i] != null)
			{
				flag &= this.func_conditions[i]();
			}
		}
		return flag;
	}

	public void Clear()
	{
	}

	public void DoActions()
	{
		for (int i = 0; i < this.func_actions.Count; i++)
		{
			if (this.func_actions[i] != null)
			{
				this.func_actions[i]();
			}
		}
	}
}

using System;
using System.Collections.Generic;

namespace Assets.MobaTools.TriggerPlugin.Scripts
{
	public interface ITriggerCreatorParam
	{
		ETriggerType2 TypeID
		{
			get;
			set;
		}

		int EventID
		{
			get;
			set;
		}

		string TriggerID
		{
			get;
			set;
		}

		List<TriggerCondition<ITriggerDoActionParam>> Func_conditions
		{
			get;
			set;
		}

		Callback<ITriggerDoActionParam> Func_actions
		{
			get;
			set;
		}
	}
}

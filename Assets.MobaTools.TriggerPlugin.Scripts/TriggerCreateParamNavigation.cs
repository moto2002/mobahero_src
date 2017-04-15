using System;
using System.Collections.Generic;

namespace Assets.MobaTools.TriggerPlugin.Scripts
{
	public class TriggerCreateParamNavigation : ITriggerCreatorParam
	{
		public ETriggerType2 TypeID
		{
			get
			{
				return ETriggerType2.TriggerEvent2_navigation;
			}
			set
			{
			}
		}

		public int EventID
		{
			get;
			set;
		}

		public string TriggerID
		{
			get;
			set;
		}

		public List<TriggerCondition<ITriggerDoActionParam>> Func_conditions
		{
			get;
			set;
		}

		public Callback<ITriggerDoActionParam> Func_actions
		{
			get;
			set;
		}
	}
}

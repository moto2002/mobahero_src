using System;

namespace Assets.MobaTools.TriggerPlugin.Scripts
{
	public class TriggerParamSkillControl : ITriggerDoActionParam
	{
		public ETriggerType2 TypeID
		{
			get
			{
				return ETriggerType2.TriggerEvent2_skillControl;
			}
		}

		public int EventID
		{
			get;
			set;
		}

		public object Trigger
		{
			get;
			set;
		}

		public string SkillID
		{
			get;
			set;
		}
	}
}

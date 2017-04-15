using System;

namespace Assets.MobaTools.TriggerPlugin.Scripts
{
	public class TriggerParamNavigation : ITriggerDoActionParam
	{
		public ETriggerType2 TypeID
		{
			get
			{
				return ETriggerType2.TriggerEvent2_navigation;
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

		public bool IsPlayer
		{
			get;
			set;
		}
	}
}

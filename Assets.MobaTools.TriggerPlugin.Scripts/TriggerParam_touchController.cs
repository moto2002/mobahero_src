using System;

namespace Assets.MobaTools.TriggerPlugin.Scripts
{
	public class TriggerParam_touchController : ITriggerDoActionParam
	{
		public ETriggerType2 TypeID
		{
			get
			{
				return ETriggerType2.TriggerEvent2_manulController;
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

		public bool Start
		{
			get;
			set;
		}
	}
}

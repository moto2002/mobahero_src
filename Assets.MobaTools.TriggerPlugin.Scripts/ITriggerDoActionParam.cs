using System;

namespace Assets.MobaTools.TriggerPlugin.Scripts
{
	public interface ITriggerDoActionParam
	{
		ETriggerType2 TypeID
		{
			get;
		}

		int EventID
		{
			get;
		}

		object Trigger
		{
			get;
		}
	}
}

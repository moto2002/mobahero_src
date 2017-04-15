using System;
using UnityEngine;

namespace Assets.MobaTools.TriggerPlugin.Scripts
{
	public class TriggerParamTouch : ITriggerDoActionParam
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

		public Vector2 Pos
		{
			get;
			set;
		}

		public int FingerID
		{
			get;
			set;
		}
	}
}

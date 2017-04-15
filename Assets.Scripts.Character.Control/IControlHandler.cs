using System;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class IControlHandler
	{
		private int ControlID;

		private Vector3 initPressPos;

		private float pressDeltaTime;

		private static float sPressDeltaTime;

		public virtual void updateControl(float deltaTime)
		{
		}

		protected void InitPressPos(Vector3 initPos)
		{
			this.initPressPos = initPos;
			this.pressDeltaTime = Time.realtimeSinceStartup;
			IControlHandler.sPressDeltaTime = Time.realtimeSinceStartup;
		}

		protected bool IsDrag(Vector3 curPos)
		{
			float num = Vector3.Distance(this.initPressPos, curPos);
			if (GlobalSettings.Instance.isLockView)
			{
				return true;
			}
			if (Time.realtimeSinceStartup - IControlHandler.sPressDeltaTime < 0.3f && num < 13f)
			{
				return false;
			}
			if (num < 0.5f)
			{
			}
			return true;
		}
	}
}

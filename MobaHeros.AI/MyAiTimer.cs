using System;
using UnityEngine;

namespace MobaHeros.AI
{
	public class MyAiTimer
	{
		private float lastUpdateTime;

		public float interval = 0.5f;

		public bool CanUpdate()
		{
			if (Time.time - this.lastUpdateTime >= this.interval)
			{
				this.lastUpdateTime = Time.time;
				return true;
			}
			return false;
		}
	}
}

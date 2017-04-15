using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class UpdateController : MonoBehaviour
	{
		public Callback OnLateUpdateCallback;

		public Callback OnUpdateCallback;

		public Callback OnFixUpdateCallback;

		public bool isActive;

		public float update_interval = 0.2f;

		private float update_time;

		public float fix_update_interval = 0.5f;

		private float fix_update_time;

		private void Start()
		{
			this.update_time = this.update_interval;
			this.fix_update_time = this.fix_update_interval;
		}

		private void OnDestroy()
		{
			this.OnLateUpdateCallback = null;
			this.OnUpdateCallback = null;
			this.OnFixUpdateCallback = null;
			this.isActive = false;
			this.update_time = this.update_interval;
			this.fix_update_time = this.fix_update_interval;
		}

		private void LateUpdate()
		{
			if (this.isActive)
			{
				this.update_time -= Time.deltaTime;
				if (this.update_time < 0f)
				{
					if (this.OnLateUpdateCallback != null)
					{
						this.OnLateUpdateCallback();
					}
					this.update_time = this.update_interval;
				}
			}
		}

		private void FixUpdate()
		{
			if (this.isActive)
			{
				this.fix_update_time -= Time.fixedDeltaTime;
				if (this.fix_update_time < 0f)
				{
					if (this.OnFixUpdateCallback != null)
					{
						this.OnFixUpdateCallback();
					}
					this.fix_update_time = this.fix_update_interval;
				}
			}
		}
	}
}

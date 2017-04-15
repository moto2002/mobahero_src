using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class TimeoutController : MonoBehaviour
	{
		public Callback OnTimeoutCallback;

		private float timeout;

		private void OnDestroy()
		{
			base.CancelInvoke();
			this.OnTimeoutCallback = null;
		}

		public void StartTimeOut(float timeout, Callback timeout_callback)
		{
			if (timeout > 0f)
			{
				this.OnTimeoutCallback = timeout_callback;
				base.Invoke("Timeout", timeout);
			}
		}

		public void StopTimeOut()
		{
			base.CancelInvoke();
			this.OnTimeoutCallback = null;
		}

		private void Timeout()
		{
			base.CancelInvoke();
			if (this.OnTimeoutCallback != null)
			{
				this.OnTimeoutCallback();
				this.OnTimeoutCallback = null;
			}
		}
	}
}

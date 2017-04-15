using Com.Game.Module;
using System;
using UnityEngine;

namespace MobaTools.Effect
{
	public class ShadowEffect
	{
		private Light _light;

		private bool _isRealTimeShadow;

		public Light Lighting
		{
			get
			{
				if (this._light == null)
				{
					this._light = CameraRoot.Instance.m_Light;
				}
				return this._light;
			}
		}

		public void ToggleShadows()
		{
			if (this._isRealTimeShadow)
			{
				this.ShowShadows(!this._isRealTimeShadow);
			}
			else
			{
				this.ShowShadows(this._isRealTimeShadow);
			}
		}

		public void ShowShadows(bool isShow)
		{
			if (this.Lighting == null)
			{
				return;
			}
			if (isShow)
			{
				this.Lighting.enabled = true;
				this._isRealTimeShadow = true;
			}
			else
			{
				this.Lighting.enabled = false;
				this._isRealTimeShadow = false;
			}
		}

		public bool IsOpenShadow()
		{
			return this._isRealTimeShadow;
		}
	}
}

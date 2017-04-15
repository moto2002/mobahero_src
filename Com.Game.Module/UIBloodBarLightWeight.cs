using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class UIBloodBarLightWeight : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_slideBloodSprite;

		[SerializeField]
		private UILabel m_hpRange;

		private Units _owner;

		private float _lastHpPercent = -1f;

		public void SetTarget(Units unit)
		{
			this._lastHpPercent = -1f;
			this._owner = unit;
			this.Update();
		}

		private void Update()
		{
			if (!this._owner)
			{
				return;
			}
			float num = this._owner.hp / this._owner.hp_max;
			if (Mathf.Abs(this._lastHpPercent - num) <= 0.01f)
			{
				return;
			}
			this._lastHpPercent = num;
			if (this.m_slideBloodSprite)
			{
				this.m_slideBloodSprite.fillAmount = num;
			}
			if (this.m_hpRange)
			{
				string text = (int)this._owner.hp + "/" + (int)this._owner.hp_max;
				this.m_hpRange.text = text;
			}
		}
	}
}
